using UnityEngine;

/// <summary>
/// Restores an object's original transform and 2D physics state.
/// Attach this component to the player or any movable puzzle object.
/// </summary>
public class ResettableTransform : MonoBehaviour, IResettable
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialScale;

    private Rigidbody2D cachedRigidbody;

    private void Awake()
    {
        // Record the object's state when the scene begins.
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialScale = transform.localScale;

        cachedRigidbody = GetComponent<Rigidbody2D>();
    }

    public void ResetState()
    {
        // Remove any remaining movement before restoring position.
        if (cachedRigidbody != null)
        {
            cachedRigidbody.linearVelocity = Vector2.zero;
            cachedRigidbody.angularVelocity = 0f;
        }

        transform.SetPositionAndRotation(
            initialPosition,
            initialRotation
        );

        transform.localScale = initialScale;
    }
}