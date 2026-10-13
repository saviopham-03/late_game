using UnityEngine;

public abstract class PuzzleInput : PuzzleObject
{
    [SerializeField] protected bool isActive = false;
    [SerializeField] protected PuzzleController puzzleController;

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