using UnityEngine;

public class ColourPickup: MonoBehaviour
{
    [SerializeField] private PlayerColour pickupColour;
    [SerializeField] private Animator _animator;
    private bool active = true;

    void Start(){
         _animator.SetFloat("anim_offset", Random.Range(0f, 1f));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!active) return;
        PlayerColourController playerColourController = other.GetComponent<PlayerColourController>();
        if(playerColourController == null || playerColourController.GetColour() == pickupColour)
        {
            return;
        }
        playerColourController.SetColour(pickupColour);
        _animator.SetTrigger("pickup");
        active = false;
    }

}
