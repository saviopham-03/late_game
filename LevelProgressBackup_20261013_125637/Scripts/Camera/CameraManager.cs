using UnityEngine;
using static System.Math;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private BoxCollider2D startingCameraPoint;
    [SerializeField] private float cameraSwapSpeed;
    private Bounds bounds;
    private bool swap = false;
    private float sizeFromHeight;
    private float sizeFromWidth;

    private void Start()
    {
        if (mainCamera != null && startingCameraPoint != null)
        {
            CloneManager.Instance.AssignStartingSpace(startingCameraPoint);
            SwitchCamera(startingCameraPoint);
        }
    }
    
    private void Update(){
        if (swap)
        {
            float new_x = Mathf.Lerp(mainCamera.transform.position.x, bounds.center.x, cameraSwapSpeed);
            float new_y = Mathf.Lerp(mainCamera.transform.position.y, bounds.center.y, cameraSwapSpeed);

            float zoom = Mathf.Lerp(mainCamera.orthographicSize, sizeFromHeight, cameraSwapSpeed);

            mainCamera.transform.position = new Vector3(
            new_x,
            new_y,
            mainCamera.transform.position.z
            );

            mainCamera.orthographicSize = zoom;
        
            if (Abs(new_x-bounds.center.x) <= 0.1 && Abs(new_x-bounds.center.x) <= 0.1 && Abs(mainCamera.orthographicSize-sizeFromHeight) <= 0.1)
            {
                mainCamera.transform.position = new Vector3(
                bounds.center.x,
                bounds.center.y,
                mainCamera.transform.position.z
                );
                mainCamera.orthographicSize = sizeFromHeight;
                swap = false;
            }
        }
    }

    public void SwitchCamera(BoxCollider2D targetPoint)
    {
        CloneManager.Instance.switchCloneSet(targetPoint);
        bounds = targetPoint.bounds;

        sizeFromHeight = bounds.size.y / 2f;
        sizeFromWidth = bounds.size.x / (2f * mainCamera.aspect);

        swap = true;

    }
}