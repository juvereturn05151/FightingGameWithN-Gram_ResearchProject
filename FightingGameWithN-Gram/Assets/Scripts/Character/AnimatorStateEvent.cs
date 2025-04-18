using UnityEngine;

public class AnimatorStateEvent : MonoBehaviour
{
    [SerializeField] private Character character;

    public void OnAttackFinished() => character.OnAttackFinished();
    public void OnHurtFinished() => character.OnHurtFinished();
    public void OnHitConfirmSuccessFinished() => character.OnHitConfirmSuccessFinished();

    public void OnThrowFinished() => character.OnThrowFinished();

    public void OnYouLoseFinished() => character.OnYouLoseFinished();

    public void OnWhiffThrowFinished() => character.OnWhiffThrowFinished();
}
