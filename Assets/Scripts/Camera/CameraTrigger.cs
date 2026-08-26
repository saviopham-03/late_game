using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private Transform leftCameraPoint;
    [SerializeField] private Transform rightCameraPoint;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            return;
        }

        if (rb.linearVelocity.x > 0.1f)
        {
            cameraManager.SwitchCamera(rightCameraPoint);
        }
        else if (rb.linearVelocity.x < -0.1f)
        {
            cameraManager.SwitchCamera(leftCameraPoint);
        }
    }
}