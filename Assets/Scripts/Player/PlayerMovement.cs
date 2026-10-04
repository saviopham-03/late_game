using System;
using UnityEngine;
using UnityEngine.InputSystem;
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
    [SerializeField] private float inputBuffer;
    [SerializeField] private Vector2 footstoolPower;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;

    private Rigidbody2D playerBody;
    private BoxCollider2D _collider;
    private float horizontalInput;
    private bool jumpRequested;
    private bool active = true;
    private bool inDialogue = false;
    public Vector2 last_vel;
    public bool IsActive => active;
    private Animator _animator;
    private float sleep_vel;
    private bool isGrounded;
    public void DisableMovement()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
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
        }
    }
    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        playerBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        moveAction.action.Enable();
        jumpAction.action.Enable();
    }

<<<<<<< HEAD
        moveAction.action.started += _ =>
        {
            Vector2 input = moveAction.action.ReadValue<Vector2>();
            horizontalInput = input.x;
=======
    private void OnEnable()
    {
        moveAction.action.started += OnMoveStarted;
        moveAction.action.canceled += OnMoveCanceled;
    }
>>>>>>> origin/dev

    private void OnDisable()
    {
        moveAction.action.started -= OnMoveStarted;
        moveAction.action.canceled -= OnMoveCanceled;
        horizontalInput = 0f;
        jumpRequested = false;
    }

<<<<<<< HEAD
        moveAction.action.canceled += _ =>
        {
            Vector2 input = moveAction.action.ReadValue<Vector2>();
            horizontalInput = input.x;
=======
    private void OnMoveStarted(InputAction.CallbackContext context)
    {
        if (!active || Time.timeScale == 0f) return;
>>>>>>> origin/dev

        Vector2 input = moveAction.action.ReadValue<Vector2>();
        horizontalInput = input.x;
        _animator.SetBool("is_running", true);
        GetComponent<SpriteRenderer>().flipX = horizontalInput != 1;
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        horizontalInput = input.x;
        _animator.SetBool("is_running", false);
    }

    private void Update()
    {
<<<<<<< HEAD
=======
        if (Time.timeScale == 0f)
        {
            jumpRequested = false;
            return;
        }
>>>>>>> origin/dev

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

        RaycastHit2D hit_l = Physics2D.Raycast(
            new Vector3(transform.position.x-GetComponent<BoxCollider2D>().bounds.extents.x, transform.position.y, transform.position.z),
            Vector2.down, 
            Mathf.Infinity, groundLayer, 0, 4);
        
        RaycastHit2D hit_r = Physics2D.Raycast(
            new Vector3(transform.position.x+GetComponent<BoxCollider2D>().bounds.extents.x, transform.position.y, transform.position.z),
            Vector2.down, 
            Mathf.Infinity, groundLayer, 0, 4);
        
        RaycastHit2D hit = hit_l.point.y >= hit_r.point.y ? hit_l : hit_r;

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

        if (groundAngle <= maxSlopeAngle && IsGrounded() && Mathf.Abs(hit.point.y-_collider.bounds.min.y)<0.1)
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