using UnityEngine;
using UnityEngine.InputSystem;

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

    private DistanceJoint2D grappleJoint;
    private LineRenderer grappleLine;
    private PlayerMovement playerMovement;

    private Collider2D currentGrapplePoint;

    private bool isGrappling;
    private bool jointActive;

    private float ropeLength;

    private void Awake()
    {
        grappleJoint = GetComponent<DistanceJoint2D>();
        grappleLine = GetComponent<LineRenderer>();
        playerMovement = GetComponent<PlayerMovement>();

        grappleAction.action.Enable();
        jumpAction.action.Enable();

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

        if (!isGrappling || currentGrapplePoint == null)
        {
            return;
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

        if (playerMovement.IsGrounded())
        {
            DetachGrapple();
        }
    }

    private Collider2D FindClosestGrapplePoint()
    {
        Collider2D[] grapplePoints = Physics2D.OverlapCircleAll(
            transform.position,
            grappleRange,
            grapplePointLayer
        );

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
        isGrappling = true;
        jointActive = false;

        currentGrapplePoint = grapplePoint;

        ropeLength = Vector2.Distance(
            transform.position,
            grapplePoint.transform.position
        );

        grappleJoint.enabled = false;
        grappleLine.enabled = true;

        if (!playerMovement.IsGrounded())
        {
            ActivateGrappleJoint();
        }

        Debug.Log(
            $"Attached to grapple point: {grapplePoint.name}"
        );
    }

    private void ActivateGrappleJoint()
    {
        grappleJoint.connectedAnchor =
            currentGrapplePoint.transform.position;

        grappleJoint.distance = ropeLength;
        grappleJoint.maxDistanceOnly = true;
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