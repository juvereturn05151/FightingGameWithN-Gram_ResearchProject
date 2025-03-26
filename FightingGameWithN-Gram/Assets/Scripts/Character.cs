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
    }
}
