using UnityEngine;

public abstract class PathModifier : PuzzleObject
{
    public abstract int GetNextNodeIndex(int currentNodeIndex, Path path);

    public override void ResetPuzzleObject()
    {
        // Child classes can override this if they have
        // traversal state that needs to be reset.
    }
}