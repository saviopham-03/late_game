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

    private Rigidbody2D playerBody;
    private DistanceJoint2D grappleJoint;
    private LineRenderer grappleLine;

    private Collider2D currentGrapplePoint;
    private bool isGrappling;

    private void Awake()
    {
        playerBody = GetComponent<Rigidbody2D>();
        grappleJoint = GetComponent<DistanceJoint2D>();
        grappleLine = GetComponent<LineRenderer>();

        grappleAction.action.Enable();

        grappleJoint.enabled = false;
        grappleLine.enabled = false;
    }

    private void Update()
    {
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
            float distance = Vector2.Distance(
                transform.position,
                grapplePoint.transform.position
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPoint = grapplePoint;
            }
        }

        return closestPoint;
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