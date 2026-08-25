using UnityEngine;
public abstract class MovementBehaviour : PuzzleObject
{
    protected Platform platform;

    public virtual void Initialise(Platform platformReference)
    {
        platform = platformReference;
    }

    public abstract bool ShouldMove();

    public virtual void OnRiderEnter(GameObject rider)
    {
        // Optional behaviour for child classes.
    }

    public virtual void OnRiderExit(GameObject rider)
    {
        // Optional behaviour for child classes.
    }

    public virtual void OnNodeReached(int nodeIndex)
    {
        // Optional behaviour for child classes.
    }

    public override void ResetPuzzleObject()
    {
        // Child classes can override this if they contain state.
    }
}