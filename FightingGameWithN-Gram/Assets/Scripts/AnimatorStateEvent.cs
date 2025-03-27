using UnityEngine;

public class AnimatorStateEvent : MonoBehaviour
{
    [SerializeField]
    private Character character;

    [SerializeField]
    private CharacterAttack characterAttack;
    public void OnAttackEnd() 
    {
        characterAttack.OnAttackEnd();
    }

    public void OnBeingHitDone()
    {
        character.OnBeingHitDone();
    }
}
