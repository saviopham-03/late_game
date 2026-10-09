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

        float playerX = other.bounds.center.x;
        float triggerX = GetComponent<Collider2D>().bounds.center.x;

        if (playerX > triggerX + 0.1f)
        {
            cameraManager.SwitchCamera(rightCameraPoint);
        }
        else if (playerX < triggerX - 0.1f)
        {
            cameraManager.SwitchCamera(leftCameraPoint);
        }
    }
}