using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private CharacterMovement characterMovement;

    [SerializeField]
    private CharacterAttack characterAttack;

    void Start()
    {
        
    }

    void Update()
    {
        UpdateAnimation();
    }

    void UpdateAnimation() 
    {
        if (animator == null) 
        {
            return;
        }

        animator.SetBool("Attack", characterAttack.IsAttack);

        if (characterAttack.IsAttack) 
        {
            return;
        }

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
}
