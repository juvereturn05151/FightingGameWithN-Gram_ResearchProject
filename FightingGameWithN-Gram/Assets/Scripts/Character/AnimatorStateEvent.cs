using UnityEngine;

public class AnimatorStateEvent : MonoBehaviour
{
    [SerializeField] private Character character;

    public void OnAttackFinished() => character.OnAttackFinished();
    //public void OnBeingHitDone() => character.OnBeingHitDone();
    //public void OnExecuteHitConfirmDone() => character.OnExecuteHitConfirmDone();
}
