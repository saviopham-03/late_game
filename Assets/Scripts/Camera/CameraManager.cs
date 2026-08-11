using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform startingCameraPoint;

    private void Start()
    {
        if (mainCamera != null && startingCameraPoint != null)
        {
            SwitchCamera(startingCameraPoint);
        }
    }

    public void SwitchCamera(Transform targetPoint)
    {
        if (mainCamera == null || targetPoint == null)
        {
            return;
        }

        mainCamera.transform.position = new Vector3(
            targetPoint.position.x,
            targetPoint.position.y,
            mainCamera.transform.position.z
        );
    }
}