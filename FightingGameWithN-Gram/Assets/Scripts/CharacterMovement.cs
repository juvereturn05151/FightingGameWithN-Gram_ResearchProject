using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;

    [SerializeField] private float moveSpeed = 5f;

    [SerializeField] private InputActionReference move;

    private Vector2 moveDirection;
    public Vector2 MoveDirection => moveDirection;

    public void MovementUpdate()
    {
        moveDirection = move.action.ReadValue<Vector2>();
    }

    public void ResetMoveDirection()
    {
        moveDirection = Vector2.zero;
        rb.velocity = Vector2.zero;
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveDirection.x * moveSpeed, 0.0f);
    }
}
