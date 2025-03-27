using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    private int playerSide;

    [SerializeField]
    private Animator animator;
    public Animator Animator=>animator;

    [SerializeField]
    private CharacterMovement characterMovement;

    [SerializeField]
    private CharacterAttack characterAttack;

    private bool isHurt;
    public bool IsHurt => isHurt;

    private bool isLose;

    private Character opponent;

    void Start()
    {
        if (playerSide == 0)
        {
            opponent = GameManager.Instance.character2;
        }
        else 
        {
            opponent = GameManager.Instance.character1;
        }

        characterAttack.Init(this);
    }

    void Update()
    {
        if (isLose) 
        {
            animator.SetBool("Lose", isLose);
            return;
        }

        characterAttack.AttackUpdate(opponent.IsHurt && opponent.Animator.GetCurrentAnimatorStateInfo(0).IsName("Hurt_Animation"));

        if (characterAttack.ExecuteHitConfirm)
        {
            opponent.SetIsLose(true);
            animator.SetBool("HitConfirm", true);
            return;
        }


        if (characterAttack.IsAttack)
        {
            characterMovement.ResetMoveDirection();
        }
        else 
        {
            characterMovement.MovementUpdate();
        }

        UpdateAnimation();
    }

    private void UpdateAnimation() 
    {
        if (animator == null) 
        {
            return;
        }

        animator.SetBool("Attack", characterAttack.IsAttack);


        if (characterMovement.MoveDirection.x >= 0.5f)
        {
            if (playerSide == 0)
            {
                animator.SetBool("MoveRight", true);
            }
            else
            {
                animator.SetBool("MoveLeft", true);
            }
        }
        else if (characterMovement.MoveDirection.x <= -0.5f)
        {
            if (playerSide == 0)
            {
                animator.SetBool("MoveLeft", true);
            }
            else
            {
                animator.SetBool("MoveRight", true);
            }
        }
        else 
        {
            animator.SetBool("MoveRight", false);
            animator.SetBool("MoveLeft", false);
        }
    }

    public void OnExecuteHitConfirmDone() 
    {
        opponent.SetIsLose(true);
    }

    public void SetIsLose(bool lose)
    {
        isLose = lose;
    }

    public void OnBeingHit() 
    {
        isHurt = true;
        characterAttack.OnAttackEnd();
        animator.SetBool("Attack", false);
        animator.SetBool("Hurt", isHurt);
    }

    public void OnBeingHitDone()
    {
        isHurt = false;
        animator.SetBool("Hurt", isHurt);
    }
}
