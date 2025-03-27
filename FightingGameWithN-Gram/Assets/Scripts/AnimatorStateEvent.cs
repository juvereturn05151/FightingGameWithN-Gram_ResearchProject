using UnityEngine;

public class AnimatorStateEvent : MonoBehaviour
{
    [SerializeField]
    private CharacterAttack characterAttack;
    public void OnAttackEnd() 
    {
        characterAttack.OnAttackEnd();
    } 
}
