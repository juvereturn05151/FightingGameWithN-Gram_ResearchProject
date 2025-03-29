using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class Character : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int playerSide;
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float throwRange = 1.9f;
    [SerializeField] private float throwForce = 200000f;

    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private AudioSource audioSource;

    [Header("References")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference attackAction;

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


    private Character opponent;

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
    float distanceFromOpponent;

    private readonly int idleHash = Animator.StringToHash("Idle");
    private readonly int walkFrontHash = Animator.StringToHash("WalkFront");
    private readonly int walkBackHash = Animator.StringToHash("WalkBack");
    private readonly int attackHash = Animator.StringToHash("Attack");
    private readonly int hurtHash = Animator.StringToHash("Hurt");
    private readonly int blockHash = Animator.StringToHash("Block");
    private readonly int throwHash = Animator.StringToHash("Throw");
    private readonly int youWinHash = Animator.StringToHash("YouWin");
    private readonly int youLoseHash = Animator.StringToHash("YouLose");
    private readonly int hitConfirmHash = Animator.StringToHash("HitConfirm");

    public Animator Animator => animator;

    private void Start()
    {
        opponent = playerSide == 0 ? GameManager.Instance.character2 : GameManager.Instance.character1;

        ResetState();
    }

    private void Update()
    {
        //if (!isReadyToFight) return;

        Debug.Log("distanceFromOpponent: " + distanceFromOpponent);

        block = false;
        animator.SetBool("Block", false);

        if (youLose)
        {
            return;
        }

        if (beingThrown)
        {
            return;
        }

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

        if (isHurt)
        {
            HandleHurtState();
            return;
        }

        if (isThrowing)
        {
            HandleThrowState();
            return;
        }

        HandleAttackState();

        if (!isAttacking && !animator.GetCurrentAnimatorStateInfo(0).IsName("Block_Animation")) 
        {
            HandleMovement();
        }

        if (isBlocking)
        {
            HandleIsBlocking();
            return;
        }
    }

    private void HandleIsBlocking() 
    {
        animator.SetBool("Block", true);
        hitBox.enabled = false;
        isBlocking = false;
    }

    private void HandleLoseState()
    {
        animator.SetBool(youLoseHash, true);

        audioSource.PlayOneShot(hurtSound);

        // Apply lose force
        float forceDirection = playerSide == 0 ? -throwForce : throwForce;
        rb.AddForce(new Vector2(forceDirection, throwForce), ForceMode2D.Impulse);
    }

    private void HandleIsAbleToHitConfirmState() 
    {
        if (!hitConfirmSuccess)
        {
            if (attackAction.action.WasPressedThisFrame())
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
            animator.SetTrigger(youWinHash);
            opponent.SetYouLose(true);

        }
    }

    public void OnThrowFinished()
    {
        if (!youWin)
        {
            youWin = true;
            audioSource.PlayOneShot(youWinSound);
            opponent.SetYouLose(true);
        }
    }

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

    private void HandleThrowState()
    {
        animator.SetBool(throwHash,true);
    }

    private void HandleAttackState()
    {
        if (isAttacking)
        {
            animator.SetBool(attackHash, true);
            return;
        }

        if (IsOpponentWithinThrowRange())
        {
            if (attackAction.action.WasPressedThisFrame())
            {
                bool movingTowardOpponent = (playerSide == 0 && moveAction.action.ReadValue<Vector2>().x > 0) ||
                                          (playerSide == 1 && moveAction.action.ReadValue<Vector2>().x < 0);

                if (movingTowardOpponent)
                {
                    ExecuteThrow();
                    return;
                }
            }
        }

        if (attackAction.action.WasPressedThisFrame())
        {
            Attack();
            return;
        }
    }

    public void OnAttackFinished() 
    {
        isAttacking = false;
        animator.SetBool(attackHash, false);
    }

    private void HandleMovement()
    {
        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();

        Vector2 movement = new Vector2(moveInput.x * movementSpeed * Time.deltaTime, 0);

        transform.Translate(movement);

        // Update animations based on movement
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

            // Check for blocking
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
            block = false;
        }
    }

    private void ExecuteThrow()
    {
        isThrowing = true;
        float xOffset = playerSide == 0 ? 1.75f : -1.75f;

        opponent.transform.position = transform.position + new Vector3(xOffset, 2f, 0);
        opponent.SetBeingThrown(true);
    }

    public void TriggerHurt()
    {
        if (block)
        {
            if (!isBlocking)
            {
                isBlocking = true;
                audioSource.PlayOneShot(guardSound);
            }

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

    public void Attack()
    {
        isAttacking = true;
        hitBox.enabled = true;
        audioSource.PlayOneShot(attackSound);
    }

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
        animator.Play(idleHash);
        rb.velocity = Vector2.zero;
    }



    private bool IsOpponentWithinThrowRange()
    {
        if (opponent == null) return false;

        distanceFromOpponent = Vector2.Distance(transform.position, opponent.transform.position);
        return distanceFromOpponent < throwRange;
    }

    // Public properties and methods
    public int PlayerSide => playerSide;
    public bool IsHurt => isHurt;
    public bool CanHitConfirm => isAbleToHitConfirm;
    public bool YouWin => youWin;
    public bool YouLose => youLose;
    public bool IsReadyToFight => isReadyToFight;
    public bool IsThrowing => isThrowing;
    public bool BeingThrown => beingThrown;

    public void SetCanHitConfirm(bool canHit) => isAbleToHitConfirm = canHit;
    public void SetYouLose(bool lose) 
    {
        youLose = lose;

        if (youLose) 
        {
            HandleLoseState();
        }
    }
    public void SetIsReadyToFight(bool ready) => isReadyToFight = ready;
    public void SetOpponent(Character opp) => opponent = opp;
    public void SetBeingThrown(bool thrown) => beingThrown = thrown;
}