using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

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