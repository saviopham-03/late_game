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
    private float grappleRange = 5f;

    [SerializeField]
    private LayerMask grapplePointLayer;

    [SerializeField]
    private LayerMask grappleObstacleLayer;

    private Rigidbody2D playerBody;
    private DistanceJoint2D grappleJoint;
    private LineRenderer grappleLine;
    private PlayerMovement playerMovement;

    private Collider2D currentGrapplePoint;
    private bool isGrappling;

    private void Awake()
    {
        playerBody = GetComponent<Rigidbody2D>();
        grappleJoint = GetComponent<DistanceJoint2D>();
        grappleLine = GetComponent<LineRenderer>();
        playerMovement = GetComponent<PlayerMovement>();

        grappleAction.action.Enable();

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

        if (grappleAction.action.triggered)
        {
            if (isGrappling)
            {
                DetachGrapple();
                return;
            }

            currentGrapplePoint = FindClosestGrapplePoint();

            if (currentGrapplePoint != null)
            {
                AttachGrapple(currentGrapplePoint);
            }
            else
            {
                Debug.Log("No grapple point in range");
            }
        }

        if (isGrappling && currentGrapplePoint != null)
        {
            if (IsGrapplePathBlocked())
            {
                DetachGrapple();
                return;
            }

            grappleLine.SetPosition(0, transform.position);
            grappleLine.SetPosition(
                1,
                currentGrapplePoint.transform.position
            );
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
        currentGrapplePoint = grapplePoint;

        grappleJoint.connectedAnchor = grapplePoint.transform.position;

        grappleJoint.distance = Vector2.Distance(
            transform.position,
            grapplePoint.transform.position
        );

        grappleJoint.enabled = true;
        grappleLine.enabled = true;

        Debug.Log($"Attached to grapple point: {grapplePoint.name}");
    }

    private void DetachGrapple()
    {
        isGrappling = false;
        currentGrapplePoint = null;

        grappleJoint.enabled = false;
        grappleLine.enabled = false;

        Debug.Log("Grapple detached");
    }
}