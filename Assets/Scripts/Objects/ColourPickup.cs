using UnityEngine;

public class ColourPickup: MonoBehaviour
{
    [SerializeField] private PlayerColour pickupColour;
    [SerializeField] private Animator _animator;
    private bool active = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!active) return;
        active = false;
        PlayerColourController playerColourController = other.GetComponent<PlayerColourController>();
        if(playerColourController == null)
        {
            return;
        }
        playerColourController.SetColour(pickupColour);
        _animator.SetTrigger("pickup");
    }

}
