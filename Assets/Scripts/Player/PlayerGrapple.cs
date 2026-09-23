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
    private InputActionReference adjustGrappleLengthAction;
    
    [SerializeField]
    private float grappleRange = 5f;

    [SerializeField]
    private float ropeAdjustSpeed = 2f;

    [SerializeField]
    private float minRopeLength = 1f;

    [SerializeField]
    private float maxRopeLength = 8f;

    [SerializeField]
    private LayerMask grapplePointLayer;

    [SerializeField]
    private LayerMask grappleObstacleLayer;
    private DistanceJoint2D grappleJoint;
    private LineRenderer grappleLine;
    private PlayerMovement playerMovement;

    private Collider2D currentGrapplePoint;

    private bool isGrappling;
    private bool jointActive;

    private float ropeLength;
    private Collider2D[] previous_grapples;

    private void Awake()
    {
        grappleJoint = GetComponent<DistanceJoint2D>();
        grappleLine = GetComponent<LineRenderer>();
        playerMovement = GetComponent<PlayerMovement>();

        grappleAction.action.Enable();
        jumpAction.action.Enable();
        adjustGrappleLengthAction.action.Enable();
        previous_grapples = new Collider2D[0];

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

        if (jumpAction.action.triggered && isGrappling)
        {
            DetachGrapple();
        }
        Collider2D[] grapples_in_range = GetGrapplesInRange();

        if (grapples_in_range != null)
        {
            foreach (Collider2D grapple in grapples_in_range)
            {
                grapple.gameObject.GetComponent<GrappleAnimatorScript>().InRange(true);
            }
            foreach (Collider2D grapple in previous_grapples)
            {
                if (Array.IndexOf(grapples_in_range, grapple) == -1) {
                    grapple.gameObject.GetComponent<GrappleAnimatorScript>().InRange(false);
                }
            }
            previous_grapples = grapples_in_range;
        }

        if (grappleAction.action.triggered)
        {
            Debug.Log(
                $"Grapple pressed | grounded={playerMovement.IsGrounded()} | y={GetComponent<Rigidbody2D>().linearVelocity.y}"
            );
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

        if (!isGrappling || currentGrapplePoint == null)
        {
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
            return;
        }

        UpdateGrappleLine();

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

    }

    private Collider2D[] GetGrapplesInRange()
    {
        Collider2D[] grapplePoints = Physics2D.OverlapCircleAll(
            transform.position,
            grappleRange,
            grapplePointLayer
        );
        return grapplePoints;
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
            Debug.Log($"Grapple candidate: {grapplePoint.name} | distance={distance}");
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

    private bool IsGrapplePathBlocked()
    {
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
        isGrappling = true;
        jointActive = false;

        currentGrapplePoint = grapplePoint;

        ropeLength = Vector2.Distance(
            transform.position,
            grapplePoint.transform.position
        );

        grappleJoint.enabled = false;
        grappleLine.enabled = true;

        ActivateGrappleJoint();

        Debug.Log(
            $"Attached to grapple point: {grapplePoint.name}"
        );
    }

    private void ActivateGrappleJoint()
    {
        grappleJoint.connectedAnchor =
            currentGrapplePoint.transform.position;

        grappleJoint.distance = ropeLength;
        grappleJoint.maxDistanceOnly = false;
        grappleJoint.enabled = true;

        jointActive = true;
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
        ropeLength = 0f;

        grappleJoint.enabled = false;
        grappleLine.enabled = false;

        Debug.Log("Grapple detached");
    }
}