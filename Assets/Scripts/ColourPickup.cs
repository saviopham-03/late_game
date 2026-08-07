using UnityEngine;

public class ColourPickup: MonoBehaviour
{
    [SerializeField] 
    private PlayerColour pickupColour;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerColourController playerColourController = other.GetComponent<PlayerColourController>();
        if(playerColourController == null)
        {
            return;
        }
        playerColourController.SetColour(pickupColour);
        Destroy(gameObject);
    }

}
