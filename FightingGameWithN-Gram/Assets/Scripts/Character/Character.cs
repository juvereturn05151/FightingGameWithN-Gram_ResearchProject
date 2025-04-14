using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public enum Actiontype
{
    Attacking = 0,
    Blocking = 1,
    Throwing = 2
}

public class Character : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int playerSide;
    [Range(1, 3)]
    [SerializeField] private int maxHealth;
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float throwRange = 1.9f;
    [SerializeField] private float throwForce = 200000f;

    [Header("AI Settings")]
    [SerializeField] private bool isAI = false;
    [SerializeField] private float aiDecisionInterval = 0.5f;

    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private AudioSource audioSource;

    [Header("References")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference attackAction;
    [SerializeField] private InputActionReference blockAction;
    [SerializeField] private InputActionReference throwAction;

    [SerializeField] private Collider2D hitBox;
    [SerializeField] private Collider2D hurtBox;
    [SerializeField] private Collider2D legHurtBox;

    [Header("Audio")]
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip guardSound;
    [SerializeField] private AudioClip hitConfirmSound;
    [SerializeField] private AudioClip youWinSound;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip throwSound;
    [SerializeField] private AudioClip whiffThrowSound;

    private Character opponent;
    private float aiTimer = 0f;
    private Vector2 aiMoveInput;
    private bool aiAttackDecision;
    private bool aiBlockDecision;
    private bool aiThrowDecision;

    private bool isAttacking;
    private bool isHurt;
    private bool isAbleToHitConfirm;
    private bool hitConfirmSuccess;
    private bool youWin;
    private bool youLose;
    private bool block;
    private bool isBlocking;
    private bool isReadyToFight;
    private bool isThrowing;
    private bool beingThrown;
    private bool hasSetOriginalPos;
    private bool holdBlock;
    private int currentHealth;
    private float distanceFromOpponent;

    private readonly int idleHash = Animator.StringToHash("Idle");
    private readonly int walkFrontHash = Animator.StringToHash("WalkFront");
    private readonly int walkBackHash = Animator.StringToHash("WalkBack");
    private readonly int attackHash = Animator.StringToHash("Attack");
    private readonly int hurtHash = Animator.StringToHash("Hurt");
    private readonly int blockHash = Animator.StringToHash("Block");
    private readonly string blockAnimation = "Block_Animation";
    private readonly string whiffThrowAnimation = "WhiffThrow_Animation";
    private readonly int throwHash = Animator.StringToHash("Throw");
    private readonly int youWinHash = Animator.StringToHash("YouWin");
    private readonly int youLoseHash = Animator.StringToHash("YouLose");
    private readonly int hitConfirmHash = Animator.StringToHash("HitConfirm");
    private readonly int whiffThrowHash = Animator.StringToHash("WhiffThrow");

    private Vector3 originalPosition;

    public event System.Action<int, int> OnHealthChanged;
    public Animator Animator => animator;



    

    public const int ACTION_LOG_SIZE = 30;
    public Queue<Actiontype> actionLog = new Queue<Actiontype>();

    private bool aiHoldingBlock = false;
    private float blockDuration = 0f;
    private float maxBlockDuration = 2.0f;

    private bool blockedSuccessfully = false;

    const float ATTACK_RANGE = 4.11f;

    private void Start()
    {
        opponent = playerSide == 0 ? GameManager.Instance.character2 : GameManager.Instance.character1;

        originalPosition = transform.position;
        hasSetOriginalPos = true;
        OnHealthChanged += UIManager.Instance.UpdatePlayerHealth;
        Init();
    }

    private void Update()
    {

        if (!isReadyToFight) return;

        if (!holdBlock)
        {
            block = false;
            animator.SetBool(blockHash, false);
        }

        if (youLose)
        {
            return;
        }

        if (beingThrown)
        {
            return;
        }

        if (isHurt)
        {
            HandleHurtState();
            return;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName(whiffThrowAnimation))
        {
            return;
        }

        if (this.isAI)
        {
            this.TryAIHitConfirm(); // Immediate follow-up if AI landed the hit
        }

        HandleAI();

        

        if (hitConfirmSuccess)
        {
            HandleHitConfirmSuccess();
            return;
        }

        if (isAbleToHitConfirm)
        {
            HandleIsAbleToHitConfirmState();
            return;
        }

        if (isThrowing)
        {
            HandleThrowState();
            return;
        }

        HandleAttackState();

        if (!isAttacking && !animator.GetCurrentAnimatorStateInfo(0).IsName(blockAnimation))
        {
            HandleMovement();
        }

        HandleGuard();

        if (isBlocking)
        {
            HandleIsBlocking();
            return;
        }
    }

    private void HandleAI() 
    {
        // AI decision making
        if (isAI)
        {
            aiTimer += Time.deltaTime;
            if (aiTimer >= aiDecisionInterval)
            {
                MakeAIDecision();
                aiTimer = 0f;
            }
        }
    }

    private void MakeAIDecision()
    {
        distanceFromOpponent = Vector2.Distance(transform.position, opponent.transform.position);
        bool isApproaching = Random.value < 0.8f;

        // Reset decisions
        aiMoveInput = Vector2.zero;
        aiAttackDecision = false;
        aiBlockDecision = false;
        aiThrowDecision = false;

        Actiontype predictedPlayerAction = N_Gram.calculateGuessedChoice(opponent.actionLog);

        Debug.Log("predictedPlayerAction: " + predictedPlayerAction);


        if (blockedSuccessfully)
        {
            if ( distanceFromOpponent <= ATTACK_RANGE * 1.5f) 
            {
                aiAttackDecision = true;
                aiBlockDecision = false;
                blockedSuccessfully = false; // Clear after punishing
                return;
            }
        }

        // 1. Continue holding block if AI decided to sit and block earlier
        if (aiHoldingBlock)
        {
            aiBlockDecision = true;
            blockDuration += Time.deltaTime;

            // 50% chance to counter after a short block at close range
            if (blockDuration > 0.3f && distanceFromOpponent < ATTACK_RANGE)
            {
                aiAttackDecision = true;
                aiHoldingBlock = false;
                blockDuration = 0f;
            }

            if (blockDuration >= maxBlockDuration)
            {
                aiHoldingBlock = false;
                blockDuration = 0f;
            }

            return;
        }

        // 1. Close-range mixups
        if (distanceFromOpponent <= throwRange)
        {
            // 30% chance to do something unexpected even when prediction says block
            if (predictedPlayerAction == Actiontype.Blocking)
            {
                float closeRangeChoice = Random.value;
                if (closeRangeChoice < 0.5f)
                {
                    aiThrowDecision = true;
                }
                else
                {
                    aiMoveInput = new Vector2(1, 0);
                }
            }
            else if(predictedPlayerAction == Actiontype.Attacking)
            {
                aiBlockDecision = true;
            }
            else if (predictedPlayerAction == Actiontype.Throwing)
            {
                aiAttackDecision = true;
            }
        }
        // 3. Footsies/movement
        else
        {
            if (distanceFromOpponent <= ATTACK_RANGE)
            {
                if (predictedPlayerAction == Actiontype.Blocking)
                {
                    aiMoveInput = new Vector2(opponent.transform.position.x > transform.position.x ? 1 : -1, 0);
                }
                else if (predictedPlayerAction == Actiontype.Attacking)
                {
                    aiBlockDecision = true;
                }
                else if (predictedPlayerAction == Actiontype.Throwing)
                {
                    aiAttackDecision = true;
                }
            }
            else 
            {
                // Dynamic movement patterns

                float movementChoice = Random.value;

                // 50% advance normally
                if (movementChoice < 0.5f)
                {
                    aiMoveInput = new Vector2(opponent.transform.position.x > transform.position.x ? 1 : -1, 0);

                    // 20% chance to attack while advancing
                    if (distanceFromOpponent < 2f && Random.value < 0.2f)
                    {
                        aiAttackDecision = true;
                    }
                }
                // 20% quick backdash
                else if (movementChoice < 0.7f)
                {
                    aiMoveInput = new Vector2(opponent.transform.position.x > transform.position.x ? -1 : 1, 0);
                }
                // 15% defensive pause (block in place)
                else if (movementChoice < 0.85f)
                {
                    aiBlockDecision = true;
                    aiMoveInput = Vector2.zero;
                }
                // 15% aggressive spam (rapid approach)
                else
                {
                    aiMoveInput = new Vector2(opponent.transform.position.x > transform.position.x ? 1.5f : -1.5f, 0);
                    if (distanceFromOpponent < 2.2f)
                    {
                        aiAttackDecision = true;
                    }
                }
            }



        }
    }

    public void TryAIHitConfirm()
    {
        if (isAI && isAbleToHitConfirm && !hitConfirmSuccess)
        {
            aiAttackDecision = true; // trigger the hit confirm
        }
    }

    private void QueueAction(Actiontype t)
    {
        actionLog.Enqueue(t);
        //Maintain an action log of a set size.
        if (actionLog.Count > ACTION_LOG_SIZE)
        {
            actionLog.Dequeue();
        }
    }

    public void Init()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(playerSide, currentHealth);
    }

    #region HitConfirm

    private void HandleIsAbleToHitConfirmState()
    {
        if (!hitConfirmSuccess)
        {
            bool attackPressed = isAI ? aiAttackDecision : attackAction.action.WasPressedThisFrame();
            if (attackPressed)
            {
                animator.SetTrigger(hitConfirmHash);
                hitConfirmSuccess = true;
                return;
            }
        }
    }

    private void HandleHitConfirmSuccess()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.1f)
        {
            audioSource.PlayOneShot(hitConfirmSound);
        }
    }

    public void OnHitConfirmSuccessFinished()
    {
        if (!youWin)
        {
            youWin = true;
            audioSource.PlayOneShot(youWinSound);
            animator.SetBool(youWinHash, true);
            opponent.SetYouLose(true);
        }
    }

    #endregion

    #region Throw

    public void OnThrowFinished()
    {
        if (!youWin)
        {
            youWin = true;
            audioSource.PlayOneShot(youWinSound);
            opponent.SetYouLose(true);
        }
    }

    private void HandleThrowState()
    {
        animator.SetBool(throwHash, true);
    }

    private void ExecuteThrow()
    {
        // Check if opponent is attacking - if so, the throw should fail
        if (opponent.IsAttacking)
        {
            // Whiff throw (attack beats throw)
            animator.SetTrigger(whiffThrowHash);
            audioSource.PlayOneShot(whiffThrowSound);
            return;
        }

        isThrowing = true;
        float xOffset = playerSide == 0 ? 1.75f : -1.75f;

        opponent.transform.position = transform.position + new Vector3(xOffset, 2f, 0);
        opponent.SetBeingThrown(true);
        audioSource.PlayOneShot(throwSound);

        if (isAI)
        {
            aiAttackDecision = false;
            aiThrowDecision = false;
        }
    }

    private bool IsOpponentWithinThrowRange()
    {
        if (opponent == null) return false;
        distanceFromOpponent = Vector2.Distance(transform.position, opponent.transform.position);
        return distanceFromOpponent < throwRange;
    }

    public void OnWhiffThrowFinished()
    {
        animator.SetBool(whiffThrowHash, false);
    }

    #endregion

    #region Hurt

    private void HandleHurtState()
    {
        animator.SetBool(hurtHash, true);
    }

    public void OnHurtFinished()
    {
        isHurt = false;
        animator.SetBool(hurtHash, false);
        isAttacking = false;
        opponent.SetCanHitConfirm(false);
    }

    public void TriggerHurt()
    {
        if (block)
        {
            if (!isBlocking)
            {
                blockedSuccessfully = true;
                isBlocking = true;
                audioSource.PlayOneShot(guardSound);
            }
            return;
        }

        if (holdBlock)
        {
            blockedSuccessfully = true;
            audioSource.PlayOneShot(guardSound);
            return;
        }

        if (youLose) return;

        animator.SetTrigger(hurtHash);
        isHurt = true;
        isAttacking = false;
        animator.SetBool(attackHash, false);
        audioSource.PlayOneShot(hitSound);
        hitBox.enabled = false;

        opponent.SetCanHitConfirm(true);
    }

    #endregion

    #region Attack

    private void HandleAttackState()
    {
        if (holdBlock) 
        {
            return;
        }

        if (isAttacking)
        {
            animator.SetBool(attackHash, true);
            return;
        }

        // Check for throw button input (new system)
        bool throwPressed = isAI ? aiThrowDecision : throwAction.action.WasPressedThisFrame();
        if (throwPressed)
        {
            QueueAction(Actiontype.Throwing);
            if (IsOpponentWithinThrowRange())
            {
                ExecuteThrow();
            }
            else
            {
                // Whiff throw (missed throw)
                animator.SetTrigger(whiffThrowHash);
                audioSource.PlayOneShot(whiffThrowSound);
            }
            return;
        }

        // Regular attack
        bool attackInput = isAI ? aiAttackDecision : attackAction.action.WasPressedThisFrame();
        if (attackInput)
        {
            Attack();
            return;
        }
    }

    public void OnAttackFinished()
    {
        isAttacking = false;
        animator.SetBool(attackHash, false);
        if (isAI) aiAttackDecision = false;
    }

    public void Attack()
    {
        // If opponent is attempting to throw, this attack should beat it
        if (opponent.IsThrowing)
        {
            opponent.OnWhiffThrowFinished(); // Cancel opponent's throw attempt
        }

        QueueAction(Actiontype.Attacking);
        isAttacking = true;
        hitBox.enabled = true;
        audioSource.PlayOneShot(attackSound);
        if (isAI) aiAttackDecision = false;
    }

    #endregion

    #region Movement

    private void HandleMovement()
    {
        Vector2 moveInput = isAI ? aiMoveInput : moveAction.action.ReadValue<Vector2>();
        Vector2 movement = new Vector2(moveInput.x * movementSpeed * Time.deltaTime, 0);
        transform.Translate(movement);

        if (Mathf.Abs(moveInput.x) > 0.1f)
        {
            bool movingRight = moveInput.x > 0;

            if (playerSide == 0)
            {
                animator.SetBool(movingRight ? walkFrontHash : walkBackHash, true);
            }
            else
            {
                animator.SetBool(movingRight ? walkBackHash : walkFrontHash, true);
            }

            if ((playerSide == 0 && !movingRight) || (playerSide == 1 && movingRight))
            {
                block = true;
                hitBox.enabled = false;
            }
        }
        else
        {
            animator.SetBool(walkFrontHash, false);
            animator.SetBool(walkBackHash, false);
            if (!holdBlock)
            {
                block = false;
            }
        }
    }

    #endregion

    #region GuardOrBlock

    private void HandleIsBlocking()
    {
        animator.SetBool(blockHash, true);
        isBlocking = false;
    }

    private void HandleGuard()
    {
        if (isAttacking) 
        {
            return;
        }

        bool blockInput = isAI ? aiBlockDecision : blockAction.action.IsPressed();
        if (blockInput)
        {
            if (!holdBlock)
            {
                QueueAction(Actiontype.Blocking);
            }
            holdBlock = true;
            HandleIsBlocking();
        }
        else
        {
            holdBlock = false;
        }
    }

    #endregion

    #region YouLose

    private void HandleLoseState()
    {
        animator.SetBool(youLoseHash, true);
        audioSource.PlayOneShot(hurtSound);

        float forceDirection = playerSide == 0 ? -throwForce : throwForce;
        rb.AddForce(new Vector2(forceDirection, throwForce), ForceMode2D.Impulse);
    }

    public void OnYouLoseFinished()
    {
        if (currentHealth <= 0)
        {
            UIManager.Instance.UpdateWinnerText(opponent.PlayerSide);
            GameManager.Instance.ChangeState(GameState.MatchEnd);
        }
        else
        {
            GameManager.Instance.ChangeState(GameState.RoundEnd);
        }
    }

    #endregion

    public void ResetState()
    {
        isAttacking = false;
        isHurt = false;
        isAbleToHitConfirm = false;
        hitConfirmSuccess = false;
        youWin = false;
        youLose = false;
        isBlocking = false;
        isReadyToFight = false;
        beingThrown = false;
        isThrowing = false;
        isReadyToFight = false;
        animator.Play(idleHash);
        animator.SetBool(walkBackHash, false);
        animator.SetBool(walkFrontHash, false);
        animator.SetBool(attackHash, false);
        animator.SetBool(hurtHash, false);
        animator.SetBool(blockHash, false);
        animator.SetBool(throwHash, false);
        animator.SetBool(youWinHash, false);
        animator.SetBool(youLoseHash, false);
        animator.SetBool(hitConfirmHash, false);

        if (hasSetOriginalPos)
        {
            this.transform.position = originalPosition;
        }

        rb.velocity = Vector2.zero;

        if (isAI)
        {
            aiMoveInput = Vector2.zero;
            aiAttackDecision = false;
            aiBlockDecision = false;
            aiThrowDecision = false;
        }
    }



    // Public properties and methods
    public int PlayerSide => playerSide;
    public bool IsHurt => isHurt;
    public bool IsAttacking => isAttacking;
    public bool CanHitConfirm => isAbleToHitConfirm;
    public bool YouWin => youWin;
    public bool YouLose => youLose;
    public bool IsReadyToFight => isReadyToFight;
    public bool IsThrowing => isThrowing;
    public bool BeingThrown => beingThrown;
    public Character Opponent => opponent;
    public float DistanceFromOpponent => distanceFromOpponent;
    public bool IsAI => isAI;

    public void SetCanHitConfirm(bool canHit) => isAbleToHitConfirm = canHit;
    public void SetYouLose(bool lose)
    {
        animator.SetBool(walkBackHash, false);
        animator.SetBool(walkFrontHash, false);
        animator.SetBool(attackHash, false);
        animator.SetBool(hurtHash, false);
        animator.SetBool(blockHash, false);
        animator.SetBool(throwHash, false);
        animator.SetBool(youWinHash, false);
        animator.SetBool(hitConfirmHash, false);

        currentHealth -= 1;
        OnHealthChanged?.Invoke(playerSide, currentHealth);
        youLose = lose;
        if (youLose) HandleLoseState();
    }
    public void SetIsReadyToFight(bool ready) => isReadyToFight = ready;
    public void SetOpponent(Character opp) => opponent = opp;
    public void SetBeingThrown(bool thrown) => beingThrown = thrown;
    public void SetIsReadyToPlay(bool isReady) => isReadyToFight = isReady;






}
