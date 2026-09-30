using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using static System.Math;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{   
    [SerializeField] private float maxSlopeAngle;
    [SerializeField] private float accelerationSpeed;
    [SerializeField] private float decelerationSpeed;
    [SerializeField] private float sleepDrift;
    [SerializeField] private float maxMoveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float coyoteTimer;
    [FormerlySerializedAs("inputBuffer")]
    [SerializeField, Min(0f)] private float inputBufferTime = 0.15f;
    [SerializeField] private Vector2 footstoolPower;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;

    private Rigidbody2D playerBody;
    private float horizontalInput;
    private bool jumpRequested;
    private float jumpBufferTimer;
    private bool active = true;
    private bool inDialogue = false;
    public Vector2 last_vel;
    public bool IsActive => active;
    private Animator _animator;
    private float sleep_vel;
    public void DisableMovement()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
        ClearJumpBuffer();
    }
    public void EnableMovement()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
    }

    public void setActive(bool active)
    {
        this.active = active;
        _animator.SetBool("is_sleeping", !active);
        sleep_vel = last_vel.x*sleepDrift;

        if (!active)
        {
            ClearJumpBuffer();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {   
        sleep_vel = playerBody.linearVelocityX*sleepDrift;
        if (!other.gameObject.CompareTag("Player")) return;
        // collided w clone

        PlayerMovement other_player = other.gameObject.GetComponent<PlayerMovement>();
        Rigidbody2D player_body = GetComponent<Rigidbody2D>();
        Rigidbody2D other_body = other.gameObject.GetComponent<Rigidbody2D>();

        Vector2 incoming_vel = other_player.last_vel;
        if (incoming_vel.y != 0 && last_vel.y != 0) { // ensuring you can't bounce on ppl
            if (other_body.position.y >= player_body.position.y) // footstooled
            {
                other_body.linearVelocityY += Abs(last_vel.y*footstoolPower.y);
                player_body.linearVelocityY = 0;
                other_body.linearVelocityX += last_vel.x*footstoolPower.x;
                player_body.linearVelocityX = 0;
            }
            // else // footstooling
            // {
            //     other_body.linearVelocityY = 0;
            //     player_body.linearVelocityY += incoming_vel.y*footstoolPower;
            // }
        }
    }
    private void Awake()
    {
        playerBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        moveAction.action.Enable();
        jumpAction.action.Enable();

        moveAction.action.started += _ =>
        {
            Vector2 input = moveAction.action.ReadValue<Vector2>();
            horizontalInput = input.x;

            _animator.SetBool("is_running", true);
            if (active) GetComponent<SpriteRenderer>().flipX = horizontalInput != 1;
        };

        moveAction.action.canceled += _ =>
        {
            Vector2 input = moveAction.action.ReadValue<Vector2>();
            horizontalInput = input.x;

            _animator.SetBool("is_running", false);
        };
    }

    private void Update()
    {
        if (!active)
        {
            ClearJumpBuffer();
            return;
        }

        if (jumpAction.action.triggered)
        {
            // Grounded jumps remain immediate. If we are airborne, remember the
            // input for a short window so it can be consumed when we land.
            if (IsGrounded())
            {
                RequestJump();
                ClearJumpBuffer();
            }
            else
            {
                jumpBufferTimer = inputBufferTime;
            }
        }

        if (jumpBufferTimer <= 0f)
        {
            return;
        }

        if (IsGrounded())
        {
            RequestJump();
            ClearJumpBuffer();
            return;
        }

        jumpBufferTimer -= Time.deltaTime;

        if (jumpBufferTimer <= 0f)
        {
            ClearJumpBuffer();
        }
    }

    private void RequestJump()
    {
        if (jumpRequested)
        {
            return;
        }

        jumpRequested = true;
        _animator.SetTrigger("jumped");
    }

    private void ClearJumpBuffer()
    {
        jumpBufferTimer = 0f;
    }

    private void FixedUpdate()
    {
        last_vel = playerBody.linearVelocity;

        RaycastHit2D hit_l = Physics2D.Raycast(
            new Vector3(transform.position.x-GetComponent<BoxCollider2D>().bounds.size.x/2, transform.position.y, transform.position.z),
            Vector2.down, 
            Mathf.Infinity, groundLayer, 0, 4);
        
        RaycastHit2D hit_r = Physics2D.Raycast(
            new Vector3(transform.position.x+GetComponent<BoxCollider2D>().bounds.size.x/2, transform.position.y, transform.position.z),
            Vector2.down, 
            Mathf.Infinity, groundLayer, 0, 4);
        
        RaycastHit2D hit = hit_l.point.y <= hit_r.point.y ? hit_l : hit_r;

        bool evenFloor = Mathf.Abs(Vector2.Angle(hit_l.normal, Vector2.up) - Vector2.Angle(hit_r.normal,Vector2.up)) <= 0.1;
        float groundAngle = Vector2.Angle(hit.normal, Vector2.up);

        if (playerBody.linearVelocity.y < 0 && !IsGrounded())
        {
            _animator.SetBool("is_falling", true);
        }
        else
        {
            _animator.SetBool("is_falling", false);
        }
        float vel_x;
        float vel_y = playerBody.linearVelocityY;

        if (active)
        {
            vel_x = Mathf.Lerp(
                playerBody.linearVelocity.x,
                maxMoveSpeed * horizontalInput,
                horizontalInput == 0 ? decelerationSpeed : accelerationSpeed
            );
        } else if (IsGrounded())
        {
            vel_x = Mathf.Lerp(
                playerBody.linearVelocity.x,
                0,
                decelerationSpeed
            );
        } else
        {
            vel_x = Mathf.Lerp(
                playerBody.linearVelocity.x,
                sleep_vel,
                decelerationSpeed
            );
        }
        // Debug.Log(vel_x);

        

        playerBody.linearVelocity = new Vector2(
            vel_x,
            vel_y
        );

        if (jumpRequested)
        {
            playerBody.linearVelocity = new Vector2(
                playerBody.linearVelocity.x,
                jumpForce
            );

            jumpRequested = false;
        }
        if (groundAngle <= maxSlopeAngle && IsGrounded())
        {
            Vector2 gravity = Physics2D.gravity * playerBody.gravityScale;

            Vector2 slopeTangent =
                new Vector2(hit.normal.y, -hit.normal.x);

            float gravityAlongSlope =
                Vector2.Dot(gravity, slopeTangent);

            Vector2 unfixedLinVel = playerBody.linearVelocity;
            playerBody.linearVelocity -= slopeTangent * gravityAlongSlope * Time.fixedDeltaTime;

            if (unfixedLinVel.magnitude <= maxMoveSpeed*0.9 && evenFloor)
            {
                playerBody.linearVelocity = Vector3.Project(playerBody.linearVelocity, slopeTangent);   
            }
        }
    }

    public bool IsGrounded()
    {
        if (groundCheck == null)
        {
            Debug.LogError("GroundCheck has not been assigned.");
            return false;
        }

        Collider2D[] groundCollider = Physics2D.OverlapCircleAll(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        foreach (Collider2D collider in groundCollider)
        {
            // dont detect self
            if (collider.transform.root == transform.root)
                continue;

            
            return true;
        }

        return false;
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