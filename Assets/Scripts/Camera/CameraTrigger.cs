using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    private CameraManager cameraManager;
    [SerializeField] private BoxCollider2D leftOrTopCameraPoint;
    [SerializeField] private BoxCollider2D rightOrBottomCameraPoint;
    [SerializeField] private bool rotate;

    private BoxCollider2D _collider;

    private void Start()
    {
        cameraManager = CameraManager.Instance;
        _collider = GetComponent<BoxCollider2D>();
    }

    private void switchCamera(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        Vector3 relativePosition = other.transform.position - _collider.bounds.center;
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            return;
        }

        if ((!rotate && relativePosition.x > 0) || (rotate && relativePosition.y < 0))
        {
            if (other.GetComponent<PlayerMovement>().IsActive) cameraManager.SwitchCamera(rightOrBottomCameraPoint);
            else
            {
                CloneManager.Instance.switchCloneSet(rightOrBottomCameraPoint, other.gameObject);
            }
            
        }
        else if ((!rotate && relativePosition.x < 0) || (rotate && relativePosition.y > 0))
        {
            if (other.GetComponent<PlayerMovement>().IsActive) cameraManager.SwitchCamera(leftOrTopCameraPoint);
            else
            {
                CloneManager.Instance.switchCloneSet(leftOrTopCameraPoint, other.gameObject);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        switchCamera(other);
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        switchCamera(other);
    }
}