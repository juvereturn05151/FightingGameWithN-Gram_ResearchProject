using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterAttack : MonoBehaviour
{
    [SerializeField] private InputActionReference attack;
    private Character character;

    public bool IsAttack { get; private set; }
    public bool ExecuteHitConfirm { get; private set; }
    public bool CanHitConfirm { get; set; }

    public void Init(Character character) => this.character = character;

    public void AttackUpdate(bool canHitConfirm)
    {
        if (canHitConfirm && attack.ToInputAction().WasPressedThisFrame())
        {
            ExecuteHitConfirm = true;
            return;
        }

        if (attack.ToInputAction().WasPressedThisFrame())
        {
            IsAttack = true;
        }
    }

    public void OnAttackEnd()
    {
        IsAttack = false;
    }
}