using UnityEngine;

public class ColourPickup : MonoBehaviour
{
    [SerializeField] private PlayerColour pickupColour;
    [SerializeField] private Animator _animator;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pickupSound;

    private bool active = true;

    void Start()
    {
        _animator.SetFloat("anim_offset", Random.Range(0f, 1f));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!active)
        {
            return;
        }

        PlayerColourController playerColourController =
            other.GetComponent<PlayerColourController>();

        if (playerColourController == null ||
            playerColourController.GetColour() == pickupColour)
        {
            return;
        }

        active = false;

        playerColourController.SetColour(pickupColour);

        // Play pickup sound
        if (audioSource != null && pickupSound != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }

        _animator.SetTrigger("pickup");
    }
}