using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterMovement characterMovement;
    [SerializeField] private CharacterAttack characterAttack;

    [Header("Settings")]
    [SerializeField] private int playerSide;

    private Character opponent;
    private bool isHurt;
    private bool isLose;

    public Animator Animator => animator;
    public CharacterAttack CharacterAttack => characterAttack;
    public bool IsHurt => isHurt;

    private void Start()
    {
        opponent = playerSide == 0 ? GameManager.Instance.character2 : GameManager.Instance.character1;
        characterAttack.Init(this);
    }

    private void Update()
    {
        if (isLose)
        {
            animator.SetBool("Lose", true);
            return;
        }

        HandleAttackState();
        HandleMovementState();
        UpdateAnimation();
    }

    private void HandleAttackState()
    {
        if (!opponent.IsHurt)
        {
            characterAttack.CanHitConfirm = false;
        }

        if (isHurt)
        {
            characterAttack.OnAttackEnd();
            animator.SetBool("Attack", false);
        }
        else
        {
            bool canHitConfirm = opponent.IsHurt && opponent.Animator.GetCurrentAnimatorStateInfo(0).IsName("Hurt_Animation");
            characterAttack.AttackUpdate(canHitConfirm);
        }

        if (characterAttack.ExecuteHitConfirm)
        {
            opponent.SetIsLose(true);
            animator.SetBool("HitConfirm", true);
        }
    }

    private void HandleMovementState()
    {
        if (characterAttack.IsAttack)
        {
            characterMovement.ResetMoveDirection();
        }
        else
        {
            characterMovement.MovementUpdate();
        }
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;

        animator.SetBool("Attack", characterAttack.IsAttack);
        UpdateMovementAnimation();
    }

    private void UpdateMovementAnimation()
    {
        bool movingRight = characterMovement.MoveDirection.x >= 0.5f;
        bool movingLeft = characterMovement.MoveDirection.x <= -0.5f;

        if (playerSide == 0)
        {
            animator.SetBool("MoveRight", movingRight);
            animator.SetBool("MoveLeft", movingLeft);
        }
        else
        {
            animator.SetBool("MoveRight", movingLeft);
            animator.SetBool("MoveLeft", movingRight);
        }
    }

    public void OnExecuteHitConfirmDone() => opponent.SetIsLose(true);
    public void SetIsLose(bool lose) => isLose = lose;

    public void OnBeingHit()
    {
        if (isHurt) return;

        opponent.characterAttack.CanHitConfirm = true;
        isHurt = true;
        characterAttack.OnAttackEnd();
        animator.SetBool("Attack", false);
        animator.SetBool("Hurt", true);
    }

    public void OnBeingHitDone()
    {
        isHurt = false;
        animator.SetBool("Hurt", false);
    }
}