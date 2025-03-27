using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    private int playerSide;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private CharacterMovement characterMovement;

    [SerializeField]
    private CharacterAttack characterAttack;

    private bool isHurt;
    public bool IsHurt => isHurt;

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
    }

    void Update()
    {
        characterAttack.AttackUpdate(opponent.IsHurt);

        if (characterAttack.ExecuteHitConfirm) 
        {
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
            animator.SetBool("MoveRight", true);
        }
        else if (characterMovement.MoveDirection.x <= -0.5f)
        {
            animator.SetBool("MoveLeft", true);
        }
        else 
        {
            animator.SetBool("MoveRight", false);
            animator.SetBool("MoveLeft", false);
        }
    }

    void ExecuteHitConfirm() 
    {
        animator.SetBool("HitConfirm", true);
    }

    public void OnBeingHit() 
    {
        isHurt = true;
        animator.SetBool("Hurt", isHurt);
    }

    public void OnBeingHitDone()
    {
        isHurt = false;
        animator.SetBool("Hurt", isHurt);
    }
}
