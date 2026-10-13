public class Loop : PathModifier
{
    public override int GetNextNodeIndex(int currentNodeIndex, Path path)
    {
        if (path.HasNextNode(currentNodeIndex))
        {
            return currentNodeIndex + 1;
        }

        return path.GetFirstNodeIndex();
    }

    public override void ResetPuzzleObject()
    {
        // No internal state to reset.
    }
}