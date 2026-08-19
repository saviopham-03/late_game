using UnityEngine;
using UnityEngine.InputSystem;
using static System.Math;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{   
    [SerializeField] private Animator _animator;
    [SerializeField] private float accelerationSpeed;
    [SerializeField] private float decelerationSpeed;
    [SerializeField] private float maxMoveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float coyoteTimer;
    [SerializeField] private float inputBuffer;
    [SerializeField] private float footstoolPower;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference grappleAction;
    [SerializeField] private InputActionReference interactAction;


    private Rigidbody2D playerBody;
    private float horizontalInput;
    private bool jumpRequested;
    private bool active = true;
    public Vector2 last_vel;
    public void setActive(bool active)
    {
        this.active = active;
        if (!this.active)
        {
            horizontalInput=0;
            _animator.SetBool("is_sleeping", true);
        }
        else
        {
            _animator.SetBool("is_sleeping", false);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        // collided w clone

        PlayerMovement other_player = other.gameObject.GetComponent<PlayerMovement>();
        Rigidbody2D other_body = other.gameObject.GetComponent<Rigidbody2D>();
        Rigidbody2D player_body = GetComponent<Rigidbody2D>();

        Vector2 incoming_vel = other_player.last_vel;
        if (incoming_vel.y != 0 && last_vel.y != 0) { // ensuring you can't bounce on ppl
            if (incoming_vel.y < 0) // footstooled
            {
                other_body.linearVelocityY += last_vel.y*footstoolPower;
                player_body.linearVelocityY = 0;
            }
            else if(incoming_vel.y > 0) // footstooling
            {
                other_body.linearVelocityY = 0;
                player_body.linearVelocityY += incoming_vel.y*footstoolPower;
            }
        }
    }
    private void Awake()
    {
        playerBody = GetComponent<Rigidbody2D>();
        moveAction.action.Enable();
        jumpAction.action.Enable();
        moveAction.action.started += ctx =>
        {
            if (active) {
                Vector2 input = moveAction.action.ReadValue<Vector2>();
                horizontalInput = input.x;
                _animator.SetBool("is_running", true);
                GetComponent<SpriteRenderer>().flipX = horizontalInput!=1;
            }
        };
        moveAction.action.canceled += ctx =>
        {
            Vector2 input = moveAction.action.ReadValue<Vector2>();
            horizontalInput = input.x;
            _animator.SetBool("is_running", false);
        };
    }

    private void Update()
    {
        if (jumpAction.action.triggered && active)
        {
            jumpRequested = IsGrounded();
            if (jumpRequested)
            {
                _animator.SetTrigger("jumped");
            }
        }
    }

    private void FixedUpdate()
    {
        last_vel = playerBody.linearVelocity;
        if (playerBody.linearVelocity.y < 0)
        {
            _animator.SetBool("is_falling", true);
        }
        else
        {
            _animator.SetBool("is_falling", false);
        }
        playerBody.linearVelocity = new Vector2(
            Mathf.Lerp(playerBody.linearVelocity.x, maxMoveSpeed*horizontalInput, horizontalInput==0?decelerationSpeed:accelerationSpeed),
            playerBody.linearVelocity.y
        );

        if (jumpRequested)
        {
            playerBody.linearVelocity = new Vector2(
                playerBody.linearVelocity.x,
                jumpForce
            );

            jumpRequested = false;
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