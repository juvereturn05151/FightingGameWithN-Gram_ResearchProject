using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterAttack : MonoBehaviour
{
    [SerializeField] private InputActionReference attack;

    private bool isAttack;

    public bool IsAttack => isAttack;

    public void AttackUpdate()
    {
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
