
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

    [Header("Grapple Detection")]
    [SerializeField] private float grappleRange = 5f;
    [SerializeField] private LayerMask grapplePointLayer;
    [SerializeField] private LayerMask grappleObstacleLayer;

    [Header("Pull Grapple")]
    [SerializeField] private float pullAcceleration = 60f;
    [SerializeField] private float pullDetachDistance = 0.5f;

    private Rigidbody2D rb;
    private DistanceJoint2D grappleJoint;
    private LineRenderer grappleLine;
    private PlayerMovement playerMovement;

    private Collider2D currentGrapplePoint;
    private GrapplePoint currentGrapple;

    private bool isGrappling;
    private bool jointActive;

    private float ropeLength;
    private Vector2 pullDirection;

    private Collider2D[] previousGrapples;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        grappleJoint = GetComponent<DistanceJoint2D>();
        grappleLine = GetComponent<LineRenderer>();
        playerMovement = GetComponent<PlayerMovement>();

        grappleAction.action.Enable();
        jumpAction.action.Enable();

        previousGrapples = Array.Empty<Collider2D>();

        grappleJoint.enabled = false;
        grappleLine.enabled = false;
    }

    private void Update()
    {
        if (!playerMovement.IsActive)
        {
            if (isGrappling)
            {
                DetachGrapple();
            }

            return;
        }

        UpdateGrappleAnimations();

        // Jumping cancels the active grapple.
        if (jumpAction.action.triggered && isGrappling)
        {
            DetachGrapple();
            return;
        }

        if (grappleAction.action.triggered)
        {
            if (isGrappling)
            {
                DetachGrapple();
                return;
            }

            Collider2D grapplePoint = FindClosestGrapplePoint();

            if (grapplePoint != null)
            {
                AttachGrapple(grapplePoint);
            }
        }

        if (!isGrappling ||
            currentGrapplePoint == null ||
            currentGrapple == null)
        {
            return;
        }

        if (IsGrapplePathBlocked())
        {
            DetachGrapple();
            return;
        }

        UpdateGrappleLine();

        // Pull grapple physics is handled in FixedUpdate.
        if (currentGrapple.Type == GrapplePoint.GrappleType.Pull)
        {
            return;
        }

        UpdateSwingGrapple();
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

    private void UpdateGrappleAnimations()
    {
        Collider2D[] grapplesInRange = GetGrapplesInRange();

        foreach (Collider2D grapple in grapplesInRange)
        {
            GrappleAnimatorScript animator =
                grapple.GetComponent<GrappleAnimatorScript>();

            if (animator != null)
            {
                animator.InRange(true);
            }
        }

        foreach (Collider2D grapple in previousGrapples)
        {
            if (grapple == null)
            {
                continue;
            }

            if (Array.IndexOf(grapplesInRange, grapple) == -1)
            {
                GrappleAnimatorScript animator =
                    grapple.GetComponent<GrappleAnimatorScript>();

                if (animator != null)
                {
                    animator.InRange(false);
                }
            }
        }

        previousGrapples = grapplesInRange;
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

        Collider2D closestPoint = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D grapplePoint in grapplePoints)
        {
            Vector2 direction =
                (Vector2)grapplePoint.transform.position -
                (Vector2)transform.position;

            float distance = direction.magnitude;

            RaycastHit2D obstacleHit = Physics2D.Raycast(
                transform.position,
                direction.normalized,
                distance,
                grappleObstacleLayer
            );

            if (obstacleHit.collider != null)
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

    private bool IsGrapplePathBlocked()
    {
        Vector2 direction =
            (Vector2)currentGrapplePoint.transform.position -
            (Vector2)transform.position;

        float distance = direction.magnitude;

        RaycastHit2D obstacleHit = Physics2D.Raycast(
            transform.position,
            direction.normalized,
            distance,
            grappleObstacleLayer
        );

        return obstacleHit.collider != null;
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
        if (currentGrapple == null ||
            currentGrapple.Type != GrapplePoint.GrappleType.Swing)
        {
            return;
        }

        grappleJoint.connectedAnchor =
            currentGrapplePoint.transform.position;

        grappleJoint.distance = ropeLength;
        grappleJoint.maxDistanceOnly = true;
        grappleJoint.enabled = true;

        jointActive = true;
    }

    private void UpdateGrappleLine()
    {
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
