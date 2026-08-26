using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(DistanceJoint2D))]
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
    private Collider2D currentGrapplePoint;
    private bool isGrappling;

    private void Awake()
    {
        playerBody = GetComponent<Rigidbody2D>();
        grappleJoint = GetComponent<DistanceJoint2D>();
        grappleAction.action.Enable();

        grappleJoint.enabled = false;
    }

    private void Update()
    {
        if (grappleAction.action.triggered)
        {
            if (isGrappling)
            {
                isGrappling = false;
                currentGrapplePoint = null;
                grappleJoint.enabled = false;

                Debug.Log("Grapple detached");
                return;
            }

            currentGrapplePoint = Physics2D.OverlapCircle(
                transform.position,
                grappleRange,
                grapplePointLayer
            );

            if (currentGrapplePoint != null)
            {
                isGrappling = true;

                grappleJoint.connectedAnchor = currentGrapplePoint.transform.position;
                grappleJoint.distance = Vector2.Distance(
                    transform.position,
                    currentGrapplePoint.transform.position
                );
                grappleJoint.enabled = true;

                Debug.Log($"Attached to grapple point: {currentGrapplePoint.name}");
            }
            else
            {
                isGrappling = false;
                Debug.Log("No grapple point in range");
            }
        }
    }
}