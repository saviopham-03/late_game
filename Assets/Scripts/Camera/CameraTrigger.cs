using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private BoxCollider2D leftOrTopCameraPoint;
    [SerializeField] private BoxCollider2D rightOrBottomCameraPoint;
    [SerializeField] private bool rotate;

    private Rigidbody2D rb;

    private void switchCamera(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        rb = other.GetComponent<Rigidbody2D>();
         
        float playerX = other.bounds.center.x;
        float triggerX = GetComponent<Collider2D>().bounds.center.x;

        if (rb == null)
        {
            return;
        }

        if ((!rotate && rb.linearVelocity.x > 0.1f) || (rotate && rb.linearVelocity.y < 0.1f))
        {
            if (other.GetComponent<PlayerMovement>().IsActive) cameraManager.SwitchCamera(rightOrBottomCameraPoint);
            else
            {
                CloneManager.Instance.switchCloneSet(rightOrBottomCameraPoint, other.gameObject);
            }
            
        }
        else if ((!rotate && rb.linearVelocity.x < -0.1f) || (rotate && rb.linearVelocity.y > 0.1f))
        {
            if (other.GetComponent<PlayerMovement>().IsActive) cameraManager.SwitchCamera(leftOrTopCameraPoint);
            else
            {
                CloneManager.Instance.switchCloneSet(leftOrTopCameraPoint, other.gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switchCamera(other);
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        switchCamera(other);
    }
}