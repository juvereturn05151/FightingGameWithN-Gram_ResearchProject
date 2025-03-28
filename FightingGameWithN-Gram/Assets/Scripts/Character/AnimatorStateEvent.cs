using UnityEngine;

public class AnimatorStateEvent : MonoBehaviour
{
    [SerializeField] private Character character;

    public void OnAttackFinished() => character.OnAttackFinished();
    public void OnHurtFinished() => character.OnHurtFinished();
    //public void OnExecuteHitConfirmDone() => character.OnExecuteHitConfirmDone();
}
