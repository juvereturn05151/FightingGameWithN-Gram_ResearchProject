using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterAttack : MonoBehaviour
{
    [SerializeField] private InputActionReference attack;

    private bool isAttack;

    public bool IsAttack => isAttack;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        isAttack = attack.ToInputAction().WasPressedThisFrame();
    }
}
