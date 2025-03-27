using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterAttack : MonoBehaviour
{
    [SerializeField] private InputActionReference attack;

    private bool isAttack;

    public bool IsAttack => isAttack;

    public void AttackUpdate()
    {
        isAttack = attack.ToInputAction().WasPressedThisFrame();
    }
}
