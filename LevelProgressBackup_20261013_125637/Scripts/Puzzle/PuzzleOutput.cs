using UnityEngine;

public abstract class PuzzleOutput : PuzzleObject
{
    [SerializeField] protected bool currentState = false;
    [SerializeField] protected bool startingState = false;
    [SerializeField] protected bool invertOutput = false;

    public void ReceiveNotification(bool puzzleSatisfied)
    {
        bool desiredState = puzzleSatisfied;

        if (invertOutput)
        {
            desiredState = !desiredState;
        }

        ChangeStatus(desiredState);
    }

    protected void ChangeStatus(bool newState)
    {
        if (currentState == newState)
        {
            return;
        }

        currentState = newState;

        if (currentState)
        {
            Activate();
        }
        else
        {
            Deactivate();
        }
    }

    protected abstract void Activate();
    protected abstract void Deactivate();

    public override void ResetPuzzleObject()
    {
        currentState = startingState;

        if (currentState)
        {
            Activate();
        }
        else
        {
            Deactivate();
        }
    }
}