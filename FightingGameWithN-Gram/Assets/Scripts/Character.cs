using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private CharacterMovement characterMovement;

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

        if (characterMovement.MoveDirection.x > 0.0001f)
        {
            animator.SetBool("MoveRight", true);
        }
        else 
        {
            animator.SetBool("MoveRight", false);
        }
    }
}
