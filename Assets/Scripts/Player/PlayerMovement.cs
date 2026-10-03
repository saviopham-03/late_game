using UnityEngine;
using UnityEngine.InputSystem;
using static System.Math;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
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
    [SerializeField] private InputActionReference grappleAction;
    [SerializeField] private InputActionReference interactAction;

    [Header("Audio")]
    [SerializeField] private AudioSource playerAudioSource;
    [SerializeField] private AudioClip landingSound;
    [SerializeField] private AudioClip footstepSound;

    [Header("Footstep Settings")]
    [SerializeField] private float footstepInterval = 0.35f;
    [SerializeField] private float minimumFootstepSpeed = 0.5f;

    private Rigidbody2D playerBody;
    private float horizontalInput;
    private bool jumpRequested;

    private bool active = true;

    public Vector2 last_vel;
    public bool IsActive => active;

    private Animator _animator;
    private float sleep_vel;

    private bool wasGrounded;
    private float footstepTimer;

    public void setActive(bool active)
    {
        this.active = active;

        _animator.SetBool("is_sleeping", !active);

        sleep_vel = last_vel.x * sleepDrift;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        sleep_vel = playerBody.linearVelocityX * sleepDrift;

        if (!other.gameObject.CompareTag("Player"))
        {
            return;
        }

        // Collided with clone
        PlayerMovement other_player =
            other.gameObject.GetComponent<PlayerMovement>();

        Rigidbody2D player_body =
            GetComponent<Rigidbody2D>();

        Rigidbody2D other_body =
            other.gameObject.GetComponent<Rigidbody2D>();

        Vector2 incoming_vel = other_player.last_vel;

        // Ensure players cannot repeatedly bounce on each other
        if (incoming_vel.y != 0 && last_vel.y != 0)
        {
            // Footstooled
            if (other_body.position.y >= player_body.position.y)
            {
                other_body.linearVelocityY +=
                    Abs(last_vel.y * footstoolPower.y);

                player_body.linearVelocityY = 0;

                other_body.linearVelocityX +=
                    last_vel.x * footstoolPower.x;

                player_body.linearVelocityX = 0;
            }
        }
    }

    private void Awake()
    {
        playerBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        moveAction.action.Enable();
        jumpAction.action.Enable();

        moveAction.action.started += ctx =>
        {
            Vector2 input =
                moveAction.action.ReadValue<Vector2>();

            horizontalInput = input.x;

            _animator.SetBool("is_running", true);

            if (active)
            {
                GetComponent<SpriteRenderer>().flipX =
                    horizontalInput != 1;
            }
        };

        moveAction.action.canceled += ctx =>
        {
            Vector2 input =
                moveAction.action.ReadValue<Vector2>();

            horizontalInput = input.x;

            _animator.SetBool("is_running", false);
        };
    }

    private void Start()
    {
        // Prevent landing sound from playing immediately
        // when the scene starts.
        wasGrounded = IsGrounded();

        // Allows the first footstep without a long delay.
        footstepTimer = footstepInterval;
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

        bool isGrounded = IsGrounded();

        // Landing sound
        if (!wasGrounded && isGrounded)
        {
            PlayLandingSound();

            // Don't play a footstep at the exact landing moment.
            footstepTimer = 0f;
        }

        wasGrounded = isGrounded;

        // Falling animation
        if (playerBody.linearVelocity.y < 0 && !isGrounded)
        {
            _animator.SetBool("is_falling", true);
        }
        else
        {
            _animator.SetBool("is_falling", false);
        }

        // Horizontal movement
        float vel_x;

        if (active)
        {
            vel_x = Mathf.Lerp(
                playerBody.linearVelocity.x,
                maxMoveSpeed * horizontalInput,
                horizontalInput == 0
                    ? decelerationSpeed
                    : accelerationSpeed
            );
        }
        else if (isGrounded)
        {
            vel_x = Mathf.Lerp(
                playerBody.linearVelocity.x,
                0,
                decelerationSpeed
            );
        }
        else
        {
            vel_x = Mathf.Lerp(
                playerBody.linearVelocity.x,
                sleep_vel,
                decelerationSpeed
            );
        }

        playerBody.linearVelocity = new Vector2(
            vel_x,
            playerBody.linearVelocity.y
        );

        // Jump
        if (jumpRequested)
        {
            playerBody.linearVelocity = new Vector2(
                playerBody.linearVelocity.x,
                jumpForce
            );

            jumpRequested = false;
        }

        // Footsteps
        HandleFootsteps(isGrounded);
    }

    private void HandleFootsteps(bool isGrounded)
    {
        bool isMoving =
            Mathf.Abs(playerBody.linearVelocity.x)
            > minimumFootstepSpeed;

        if (active && isGrounded && isMoving)
        {
            footstepTimer += Time.fixedDeltaTime;

            if (footstepTimer >= footstepInterval)
            {
                PlayFootstepSound();
                footstepTimer = 0f;
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }

    private void PlayLandingSound()
    {
        if (playerAudioSource != null &&
            landingSound != null)
        {
            playerAudioSource.PlayOneShot(
                landingSound
            );
        }
    }

    private void PlayFootstepSound()
    {
        if (playerAudioSource != null &&
            footstepSound != null)
        {
            playerAudioSource.PlayOneShot(
                footstepSound
            );
        }
    }

    public bool IsGrounded()
    {
        if (groundCheck == null)
        {
            Debug.LogError(
                "GroundCheck has not been assigned."
            );

            return false;
        }

        Collider2D[] groundCollider =
            Physics2D.OverlapCircleAll(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            );

        foreach (Collider2D collider in groundCollider)
        {
            // Don't detect self.
            if (collider.transform.root == transform.root)
            {
                continue;
            }

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