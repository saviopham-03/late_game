using UnityEngine;

[DisallowMultipleComponent]
public class InteractionAudioFeedback : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clip;

    public void Play()
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void Reset()
    {
        audioSource = GetComponent<AudioSource>();
    }
}
