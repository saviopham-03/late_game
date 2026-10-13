using UnityEngine;

public class RideActivatedMovement : MovementBehaviour
{
    private bool journeyRequested = false;
    private bool canTrigger = true;

    public override bool ShouldMove()
    {
        return journeyRequested;
    }

    public override void OnRiderEnter(GameObject rider)
    {
        if (!canTrigger)
        {
            return;
        }

        journeyRequested = true;
        canTrigger = false;
    }

    public override void OnNodeReached(int nodeIndex)
    {
        journeyRequested = false;
    }

    public override void OnRiderExit(GameObject rider)
    {
        if (platform.GetRiderCount() == 0)
        {
            canTrigger = true;
        }
    }

    public override void ResetPuzzleObject()
    {
        journeyRequested = false;
        canTrigger = true;
    }
}