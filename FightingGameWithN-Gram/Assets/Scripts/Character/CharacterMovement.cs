using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private InputActionReference move;

    public Vector2 MoveDirection { get; private set; }

    public void MovementUpdate() => MoveDirection = move.action.ReadValue<Vector2>();

    public void ResetMoveDirection()
    {
        MoveDirection = Vector2.zero;
        rb.velocity = Vector2.zero;
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(MoveDirection.x * moveSpeed, 0f);
    }
}
