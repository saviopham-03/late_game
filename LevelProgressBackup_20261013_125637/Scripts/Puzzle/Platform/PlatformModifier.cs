using UnityEngine;

public abstract class PlatformModifier : PuzzleObject
{
    protected Platform platform;

    public virtual void Initialise(Platform platformReference)
    {
        platform = platformReference;
    }

    public virtual void OnRiderEnter(GameObject rider)
    {
        // Optional.
    }

    public virtual void OnRiderExit(GameObject rider)
    {
        // Optional.
    }

    public virtual void OnNodeReached(int nodeIndex)
    {
        // Optional.
    }

    public virtual void OnPuzzleMovementChanged(bool movementAllowed)
    {
        // Optional.
    }

    public virtual void UpdateModifier()
    {
        // Optional.
    }

    public virtual bool BlocksMovement()
    {
        return false;
    }

    public override void ResetPuzzleObject()
    {
        // Optional child reset behaviour.
    }
}