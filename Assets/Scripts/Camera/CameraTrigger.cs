using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    public enum TriggerDirection
    {
        Left,
        Right
    }

    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private Transform targetCameraPoint;
    [SerializeField] private TriggerDirection requiredDirection;

    private void OnTriggerEnter2D(Collider2D other)
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

        if (requiredDirection == TriggerDirection.Right &&
            rb.linearVelocity.x > 0)
        {
            cameraManager.SwitchCamera(targetCameraPoint);
        }

        if (requiredDirection == TriggerDirection.Left &&
            rb.linearVelocity.x < 0)
        {
            cameraManager.SwitchCamera(targetCameraPoint);
        }
    }
}