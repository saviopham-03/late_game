using UnityEngine;

public class PlatformRiderTrigger : MonoBehaviour
{
    [SerializeField] private Platform platform;

    [Header("Rider Detection")]
    [SerializeField] private bool requirePlayerMovement = true;
    [SerializeField] private string riderTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject rider = GetRiderObject(other);

        if (rider == null)
        {
            return;
        }

        platform.RiderEnter(rider);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        GameObject rider = GetRiderObject(other);

        if (rider == null)
        {
            return;
        }

        platform.RiderExit(rider);
    }

    private GameObject GetRiderObject(Collider2D other)
    {
        if (requirePlayerMovement)
        {
            PlayerMovement movement =
                other.GetComponentInParent<PlayerMovement>();

            if (movement == null)
            {
                return null;
            }

            return movement.gameObject;
        }

        if (!other.CompareTag(riderTag))
        {
            return null;
        }

        return other.gameObject;
    }
}