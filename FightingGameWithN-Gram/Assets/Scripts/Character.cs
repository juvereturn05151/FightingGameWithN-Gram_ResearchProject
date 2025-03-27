using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private CharacterMovement characterMovement;

    [SerializeField]
    private CharacterAttack characterAttack;

    private bool isHurt;

    void Start()
    {
        
    }

    void Update()
    {
        characterAttack.AttackUpdate();

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
