using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D playerBody;
    private float horizontalInput;
    private bool jumpRequested;

    private void Awake()
    {
        playerBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        horizontalInput = 0f;

        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.aKey.isPressed ||
            Keyboard.current.leftArrowKey.isPressed)
        {
            horizontalInput -= 1f;
        }

        if (Keyboard.current.dKey.isPressed ||
            Keyboard.current.rightArrowKey.isPressed)
        {
            horizontalInput += 1f;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log("Space detected");

            bool grounded = IsGrounded();
            Debug.Log("Grounded: " + grounded);

            if (grounded)
            {
                jumpRequested = true;
            }
        }
    }

    private void FixedUpdate()
    {
        playerBody.linearVelocity = new Vector2(
            horizontalInput * moveSpeed,
            playerBody.linearVelocity.y
        );

        if (jumpRequested)
        {
            playerBody.linearVelocity = new Vector2(
                playerBody.linearVelocity.x,
                jumpForce
            );

            jumpRequested = false;
            Debug.Log("Jump performed");
        }
    }

    private bool IsGrounded()
    {
        if (groundCheck == null)
        {
            Debug.LogError("GroundCheck has not been assigned.");
            return false;
        }

        Collider2D groundCollider = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        return groundCollider != null;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundCheckRadius
        );
    }
}