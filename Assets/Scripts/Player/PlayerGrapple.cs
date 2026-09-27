using UnityEngine;
using UnityEngine.InputSystem;
using System;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(DistanceJoint2D))]
[RequireComponent(typeof(LineRenderer))]
public class PlayerGrapple : MonoBehaviour
{
    [SerializeField]
    private InputActionReference grappleAction;

    [SerializeField]
    private InputActionReference jumpAction;

    [SerializeField]
    private float grappleRange = 5f;

    [SerializeField]
    private LayerMask grapplePointLayer;

    [SerializeField]
    private LayerMask grappleObstacleLayer;

    [Header("Pull Grapple")]
    [SerializeField]
    private float pullAcceleration = 60f;

    [SerializeField]
    private float pullDetachDistance = 0.5f;

    private Rigidbody2D rb;
    private DistanceJoint2D grappleJoint;
    private LineRenderer grappleLine;
    private PlayerMovement playerMovement;

    private Collider2D currentGrapplePoint;
    private GrapplePoint currentGrapple;

    private bool isGrappling;
    private bool jointActive;

    private float ropeLength;

    // Direction is calculated once when the pull grapple begins.
    // This keeps the player's trajectory perfectly linear.
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

        previousGrapples = new Collider2D[0];

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
            else
            {
                Debug.Log("No grapple point in range");
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

        // Pull grapple movement is handled in FixedUpdate.
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
            (Vector2)currentGrapplePoint.transform.position -
            rb.position;

        // Measures how far the player still has to travel
        // along the original grapple direction.
        float distanceAlongPath =
            Vector2.Dot(
                toGrapple,
                pullDirection
            );

        // Once we reach/pass the grapple point, detach.
        // We deliberately leave linearVelocity unchanged.
        if (distanceAlongPath <= pullDetachDistance)
        {
            DetachGrapple();
            return;
        }

        // Find how quickly we're already travelling
        // along the intended launch direction.
        float speedAlongDirection =
            Vector2.Dot(
                rb.linearVelocity,
                pullDirection
            );

        // Don't let velocity opposite the grapple direction
        // fight against the pull.
        speedAlongDirection =
            Mathf.Max(
                speedAlongDirection,
                0f
            );

        // Continuously accelerate.
        // There is deliberately NO maximum speed.
        speedAlongDirection +=
            pullAcceleration * Time.fixedDeltaTime;

        // Force the player's velocity to remain on the
        // original player -> grapple trajectory.
        rb.linearVelocity =
            pullDirection * speedAlongDirection;
    }

    private void UpdateGrappleAnimations()
    {
        Collider2D[] grapplesInRange =
            GetGrapplesInRange();

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

            if (Array.IndexOf(
                grapplesInRange,
                grapple
            ) == -1)
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
        float currentDistance =
            Vector2.Distance(
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
        Collider2D[] grapplePoints =
            GetGrapplesInRange();

        Collider2D closestPoint = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D grapplePoint in grapplePoints)
        {
            Vector2 direction =
                (Vector2)grapplePoint.transform.position -
                (Vector2)transform.position;

            float distance =
                direction.magnitude;

            RaycastHit2D obstacleHit =
                Physics2D.Raycast(
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

        float distance =
            direction.magnitude;

        RaycastHit2D obstacleHit =
            Physics2D.Raycast(
                transform.position,
                direction.normalized,
                distance,
                grappleObstacleLayer
            );

        return obstacleHit.collider != null;
    }

    private void AttachGrapple(Collider2D grapplePoint)
    {
        GrapplePoint point =
            grapplePoint.GetComponent<GrapplePoint>();

        if (point == null)
        {
            Debug.LogWarning(
                $"Grapple point {grapplePoint.name} " +
                "has no GrapplePoint component."
            );

            return;
        }

        isGrappling = true;
        jointActive = false;

        currentGrapplePoint = grapplePoint;
        currentGrapple = point;

        grappleJoint.enabled = false;
        grappleLine.enabled = true;

        if (currentGrapple.Type ==
            GrapplePoint.GrappleType.Swing)
        {
            ropeLength =
                Vector2.Distance(
                    transform.position,
                    grapplePoint.transform.position
                );

            if (!playerMovement.IsGrounded())
            {
                ActivateGrappleJoint();
            }
        }
        else if (currentGrapple.Type ==
                 GrapplePoint.GrappleType.Pull)
        {
            // Calculate direction ONCE.
            //
            // This works regardless of which side
            // of the orb the player starts on.
            pullDirection =
                (
                    (Vector2)grapplePoint.transform.position -
                    rb.position
                ).normalized;

            ropeLength =
                Vector2.Distance(
                    rb.position,
                    grapplePoint.transform.position
                );

            // Preserve velocity that is already travelling
            // towards the grapple.
            float existingSpeed =
                Vector2.Dot(
                    rb.linearVelocity,
                    pullDirection
                );

            existingSpeed =
                Mathf.Max(
                    existingSpeed,
                    0f
                );

            // Remove perpendicular/opposing velocity so
            // the pull starts on the correct linear path.
            rb.linearVelocity =
                pullDirection * existingSpeed;
        }

        Debug.Log(
            $"Attached to {point.Type} grapple point: " +
            $"{grapplePoint.name}"
        );
    }

    private void ActivateGrappleJoint()
    {
        if (currentGrapple == null ||
            currentGrapple.Type !=
            GrapplePoint.GrappleType.Swing)
        {
            return;
        }

        grappleJoint.connectedAnchor =
            currentGrapplePoint.transform.position;

        grappleJoint.distance =
            ropeLength;

        grappleJoint.maxDistanceOnly =
            true;

        grappleJoint.enabled =
            true;

        jointActive =
            true;
    }

    private void UpdateGrappleLine()
    {
        grappleLine.SetPosition(
            0,
            transform.position
        );

        grappleLine.SetPosition(
            1,
            currentGrapplePoint.transform.position
        );
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

        // IMPORTANT:
        // Do not modify rb.linearVelocity here.
        //
        // Whatever velocity was generated by the pull
        // becomes the player's launch velocity.

        Debug.Log("Grapple detached");
    }
}