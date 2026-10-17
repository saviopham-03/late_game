using UnityEngine;

public abstract class PuzzleInput : PuzzleObject
{
    [SerializeField] protected bool isActive = false;
    [SerializeField] protected PuzzleController puzzleController;

    private InteractionAudioFeedback pickupAudio;

    void Start()
    {
        GetPickupAudio();
    }

    protected void GetPickupAudio()
    {
        pickupAudio = GetComponent<InteractionAudioFeedback>();
    }

    protected void PlaySound() {
        if (pickupAudio != null) pickupAudio.Play();
        }

    public void SetActive(bool newState)
    {
        if (isActive == newState)
        {
            return;
        }

        isActive = newState;
        SendNotification();
    }

    public bool GetCondition()
    {
        return isActive;
    }

    protected void SendNotification()
    {
        if (puzzleController != null)
        {
            puzzleController.InputChanged();
        }
    }
}