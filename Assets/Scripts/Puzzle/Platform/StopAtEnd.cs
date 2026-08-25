public class StopAtEnd : PathModifier
{
    public override int GetNextNodeIndex(int currentNodeIndex, Path path)
    {
        if (path.HasNextNode(currentNodeIndex))
        {
            return currentNodeIndex + 1;
        }

        return -1;
    }

    public override void ResetPuzzleObject()
    {
        // No internal state to reset.
    }
}