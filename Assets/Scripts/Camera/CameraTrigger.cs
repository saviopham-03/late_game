using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private BoxCollider2D leftCameraPoint;
    [SerializeField] private BoxCollider2D rightCameraPoint;

    private void switchCamera(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        float playerX = other.bounds.center.x;
        float triggerX = GetComponent<Collider2D>().bounds.center.x;

        if (playerX > triggerX + 0.1f)
        {
            if (other.GetComponent<PlayerMovement>().IsActive) cameraManager.SwitchCamera(rightCameraPoint);
            else
            {
                CloneManager.Instance.switchCloneSet(rightCameraPoint, other.gameObject);
            }
            
        }
        else if (playerX < triggerX - 0.1f)
        {
            if (other.GetComponent<PlayerMovement>().IsActive) cameraManager.SwitchCamera(leftCameraPoint);
            else
            {
                CloneManager.Instance.switchCloneSet(leftCameraPoint, other.gameObject);
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