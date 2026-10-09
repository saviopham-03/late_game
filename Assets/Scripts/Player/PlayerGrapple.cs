
using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(DistanceJoint2D))]
[RequireComponent(typeof(LineRenderer))]
public class PlayerGrapple : MonoBehaviour
{
    [Header("Grapple Input")]
    [SerializeField] private InputActionReference grappleAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference adjustGrappleLengthAction;

    [SerializeField] private InputActionReference toggleGrappleModeAction;

    [Header("Grapple Detection")]
    [SerializeField] private float grappleRange = 5f;
    [SerializeField] private LayerMask grapplePointLayer;
    [SerializeField] private LayerMask grappleObstacleLayer;

    [Header("Pull Grapple")]
    [SerializeField] private float pullAcceleration = 60f;
    [SerializeField] private float pullDetachDistance = 0.5f;

    [Header("Swing Grapple")]
    [SerializeField] private float swingPumpForce = 8f;
    [SerializeField] private float maxSwingSpeed = 18f;

    [SerializeField]
    private float ropeAdjustSpeed = 2f;

    [SerializeField]
    private float minRopeLength = 1f;

    [SerializeField]
    private float maxRopeLength = 8f;

    [SerializeField]
    private GrappleInputMode grappleInputMode = GrappleInputMode.Toggle;
    private DistanceJoint2D grappleJoint;
    private LineRenderer grappleLine;
    private PlayerMovement playerMovement;

    private Collider2D currentGrapplePoint;
    private GrapplePoint currentGrapple;

    private bool isGrappling;
    private bool jointActive;
    private Rigidbody2D rb;

    private float ropeLength;
    private Vector2 pullDirection;

    private Collider2D[] previousGrapples;

    private Collider2D highlightedGrapplePoint;
    private Collider2D closestPoint;

    public bool IsGrappling => isGrappling;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        grappleJoint = GetComponent<DistanceJoint2D>();
        grappleLine = GetComponent<LineRenderer>();
        playerMovement = GetComponent<PlayerMovement>();

        grappleAction.action.Enable();
        jumpAction.action.Enable();
        adjustGrappleLengthAction.action.Enable();
        toggleGrappleModeAction.action.Enable();
        previousGrapples = Array.Empty<Collider2D>();

        grappleJoint.enabled = false;
        grappleLine.enabled = false;
    }

  private void Update()
    {
        if (Time.timeScale == 0f) return;
        closestPoint = FindClosestGrapplePoint();

        if (!playerMovement.IsActive)
        {
            // HighlightGrapplePoint(null);

            if (isGrappling)
            {
                DetachGrapple();
            }

            return;
        }

        // UpdateGrappleAnimations();
        HighlightGrapplePoint(isGrappling ? currentGrapplePoint : closestPoint);

        if (toggleGrappleModeAction.action.WasPressedThisFrame())
        {
            grappleInputMode =
                grappleInputMode == GrappleInputMode.Toggle
                    ? GrappleInputMode.Hold
                    : GrappleInputMode.Toggle;

            Debug.Log($"Grapple input mode: {grappleInputMode}");
        }
        if (grappleInputMode == GrappleInputMode.Toggle)
        {
            if (grappleAction.action.WasPressedThisFrame())
            {
                if (isGrappling)
                {
                    DetachGrapple();
                    return;
                }

                if (closestPoint != null)
                {
                    AttachGrapple(closestPoint);
                }
                // else
                // {
                //     Debug.Log("No grapple point in range");
                // }
            }
        }
        else if (grappleInputMode == GrappleInputMode.Hold)
        {
            if (grappleAction.action.WasPressedThisFrame())
            {
                if (closestPoint != null)
                {
                    AttachGrapple(closestPoint);
                }            
            }

            if (grappleAction.action.WasReleasedThisFrame() && isGrappling)
            {
                DetachGrapple();
            }
        }

        // Jumping cancels the active grapple.
        if (jumpAction.action.triggered && isGrappling)
        {
            DetachGrapple();
            return;
        }

        float ropeInput = adjustGrappleLengthAction.action.ReadValue<float>();

        if (ropeInput != 0f)
        {
            ropeLength -= ropeInput * ropeAdjustSpeed * Time.deltaTime;
            ropeLength = Mathf.Clamp(ropeLength, minRopeLength, maxRopeLength);
            grappleJoint.distance = ropeLength;
        }

        if (IsGrapplePathBlocked())
        {
            DetachGrapple();
            // HighlightGrapplePoint(closestPoint);
            return;
        }

        if (isGrappling)
        {
            UpdateGrappleLine();

            // Pull grapple physics is handled in FixedUpdate.
            if (currentGrapple.Type == GrapplePoint.GrappleType.Pull)
            {
                return;
            }

            UpdateSwingGrapple();
        }

        
    }

    private void FixedUpdate()
    {
        if (!isGrappling ||
            currentGrapplePoint == null ||
            currentGrapple == null)
        {
            return;
        }

        if (currentGrapple.Type == GrapplePoint.GrappleType.Pull)
        {
            UpdatePullGrapple();
        } else
        {
            ApplySwingControl(playerMovement.HorizontalInput);
        }
    }

    private void UpdatePullGrapple()
    {
        Vector2 toGrapple =
            (Vector2)currentGrapplePoint.transform.position - rb.position;

        // Measure the remaining distance along the original pull path.
        float distanceAlongPath = Vector2.Dot(
            toGrapple,
            pullDirection
        );

        // Detach near the orb without modifying the player's velocity.
        if (distanceAlongPath <= pullDetachDistance)
        {
            DetachGrapple();
            return;
        }

        // Determine the current speed along the pull direction.
        float speedAlongDirection = Vector2.Dot(
            rb.linearVelocity,
            pullDirection
        );

        // Prevent movement opposite to the pull direction.
        speedAlongDirection = Mathf.Max(speedAlongDirection, 0f);

        // Accelerate towards the grapple point.
        speedAlongDirection += pullAcceleration * Time.fixedDeltaTime;

        Vector2 new_vel = pullDirection * speedAlongDirection;

        Vector2 cap_vel = new Vector2(new_vel.x, Mathf.Clamp(new_vel.y, new_vel.y, playerMovement.jumpForce*1.5f));

        // Maintain the original straight-line trajectory.
        rb.linearVelocity = cap_vel;
    }

    private void ApplySwingControl(float input)
{
    if (!jointActive || currentGrapple == null || currentGrapple.Type != GrapplePoint.GrappleType.Swing) return;
    
    Vector2 ropeDirection = (rb.position - (Vector2)currentGrapplePoint.transform.position).normalized;
    Vector2 tangent = new Vector2(-ropeDirection.y,ropeDirection.x);

    rb.AddForce(tangent * input * swingPumpForce);
    rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity,maxSwingSpeed);
}

    private void UpdateSwingGrapple()
    {
        float currentDistance = Vector2.Distance(
            transform.position,
            currentGrapplePoint.transform.position
        );

        if (!jointActive)
        {
            if (playerMovement.IsGrounded() &&
                currentDistance > grappleRange)
            {
                DetachGrapple();
                // HighlightGrapplePoint(closestPoint);
                return;
            }

            if (!playerMovement.IsGrounded() &&
                currentDistance >= ropeLength)
            {
                ActivateGrappleJoint();
            }

            return;
        }

        if (playerMovement.IsGrounded())
        {
            DetachGrapple();
            // HighlightGrapplePoint(closestPoint);
        }
    }

    private Collider2D[] GetGrapplesInRange()
    {
        return Physics2D.OverlapCircleAll(
            transform.position,
            grappleRange,
            grapplePointLayer
        );
    }

    private Collider2D FindClosestGrapplePoint()
    {
        Collider2D[] grapplePoints = GetGrapplesInRange();

        closestPoint = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D grapplePoint in grapplePoints)
        {
            Vector2 direction =
                (Vector2)grapplePoint.transform.position -
                (Vector2)transform.position;

            float distance = direction.magnitude;
            if (distance > grappleRange)
            {
                continue;
            }

            RaycastHit2D[] obstacleHits = Physics2D.RaycastAll(
                transform.position,
                direction.normalized,
                distance,
                grappleObstacleLayer
            );

            bool pathBlocked = false;

            foreach (RaycastHit2D hit in obstacleHits)
            {
                if (hit.collider.transform.root == transform.root)
                {
                    continue;
                }

                pathBlocked = true;
                break;
            }

            if (pathBlocked)
            {
                continue;
            }
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPoint = grapplePoint;
            }
        }

        return closestPoint;
    }

    private void HighlightGrapplePoint(Collider2D point)
    {
        if (highlightedGrapplePoint == point)
        {
            return;
        }

        if (highlightedGrapplePoint != null)
        {
            highlightedGrapplePoint.GetComponent<GrappleAnimatorScript>().InRange(false);
        }

        highlightedGrapplePoint = point;

        if (highlightedGrapplePoint != null)
        {
            highlightedGrapplePoint.GetComponent<GrappleAnimatorScript>().InRange(true);
        }
    }

    private bool IsGrapplePathBlocked()
    {
        if (!currentGrapplePoint) return false;

        Vector2 direction =
            (Vector2)currentGrapplePoint.transform.position -
            (Vector2)transform.position;

        float distance = direction.magnitude;

        RaycastHit2D[] obstacleHits = Physics2D.RaycastAll(
            transform.position,
            direction.normalized,
            distance,
            grappleObstacleLayer
        );

        foreach (RaycastHit2D hit in obstacleHits)
        {
            // Ignore the player's own colliders
            if (hit.collider.transform.root == transform.root)
            {
                continue;
            }

            return true;
        }

        return false;
    }

    private void AttachGrapple(Collider2D grapplePoint)
    {
        GrapplePoint point = grapplePoint.GetComponent<GrapplePoint>();

        if (point == null)
        {
            Debug.LogWarning(
                $"Grapple point {grapplePoint.name} has no GrapplePoint component."
            );
            return;
        }

        isGrappling = true;
        jointActive = false;

        currentGrapplePoint = grapplePoint;
        currentGrapple = point;

        grappleJoint.enabled = false;
        grappleLine.enabled = true;

        if (currentGrapple.Type == GrapplePoint.GrappleType.Swing)
        {
            ropeLength = Vector2.Distance(
                transform.position,
                grapplePoint.transform.position
            );

            if (!playerMovement.IsGrounded())
            {
                ActivateGrappleJoint();
            }
        }
        else if (currentGrapple.Type == GrapplePoint.GrappleType.Pull)
        {
            // Calculate the pull direction once at attachment.
            pullDirection = (
                (Vector2)grapplePoint.transform.position - rb.position
            ).normalized;

            ropeLength = Vector2.Distance(
                rb.position,
                grapplePoint.transform.position
            );

            // Preserve the player's speed when redirecting towards the orb.
            float existingSpeed = rb.linearVelocity.magnitude;

            rb.linearVelocity = pullDirection * existingSpeed;
        }
    }

    private void ActivateGrappleJoint()
    {
        if (!currentGrapplePoint) return;

        if (currentGrapple == null ||
            currentGrapple.Type != GrapplePoint.GrappleType.Swing)
        {
            return;
        }

        grappleJoint.connectedAnchor =
            currentGrapplePoint.transform.position;

        grappleJoint.distance = ropeLength;
        grappleJoint.maxDistanceOnly = false;
        grappleJoint.enabled = true;

        jointActive = true;
    }

    private void UpdateGrappleLine()
    {
        if (!currentGrapplePoint) return;
        grappleLine.SetPosition(0, transform.position);
        grappleLine.SetPosition(1, currentGrapplePoint.transform.position);
    }

    private void DetachGrapple()
    {
        isGrappling = false;
        jointActive = false;

        currentGrapplePoint = null;
        currentGrapple = null;

        ropeLength = 0f;
        pullDirection = Vector2.zero;

        grappleJoint.enabled = false;
        grappleLine.enabled = false;

        // Preserve Rigidbody2D.linearVelocity so the player
        // retains their current momentum after detachment.
    }
}
