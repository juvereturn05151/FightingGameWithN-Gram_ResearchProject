using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterAttack : MonoBehaviour
{
    [SerializeField] 
    private InputActionReference attack;

    [SerializeField]
    private Character character;

    private bool isAttack;

    public bool IsAttack => isAttack;

    private bool executeHitConfirm;

    public bool ExecuteHitConfirm => executeHitConfirm;

    public bool CanHitConfirm;

    public void Init(Character character) 
    {
        this.character = character;
    }

    public void AttackUpdate(bool canHitConfirm)
    {
        if (canHitConfirm) 
        {
            if (attack.ToInputAction().WasPressedThisFrame())
            {
                executeHitConfirm = true;
                return;
            }
        }

        if (attack.ToInputAction().WasPressedThisFrame())
        {
            isAttack = true;
        }
    }

    public void OnAttackEnd() 
    {
        isAttack = false;
    }
}
