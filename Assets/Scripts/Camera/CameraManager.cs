using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private BoxCollider2D startingCameraPoint;

    private void Start()
    {
        if (mainCamera != null && startingCameraPoint != null)
        {
            SwitchCamera(startingCameraPoint);
        }
    }

    public void SwitchCamera(BoxCollider2D targetPoint)
    {
        CloneManager.Instance.switchCloneSet(targetPoint);
        Bounds bounds = targetPoint.bounds;

        // Center camera on rectangle
        mainCamera.transform.position = new Vector3(
            bounds.center.x,
            bounds.center.y,
            mainCamera.transform.position.z
        );

        // Calculate orthographic size
        float height = bounds.size.y;
        float width = bounds.size.x;

        float aspect = mainCamera.aspect;

        float sizeFromHeight = height / 2f;
        float sizeFromWidth = width / (2f * aspect);

        mainCamera.orthographicSize = Mathf.Max(
            sizeFromHeight,
            sizeFromWidth
        );
    }
}