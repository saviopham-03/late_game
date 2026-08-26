public class PingPong : PathModifier
{
    private enum Direction
    {
        Forward,
        Backward
    }

    private Direction direction = Direction.Forward;

    public override int GetNextNodeIndex(int currentNodeIndex, Path path)
    {
        if (direction == Direction.Forward)
        {
            if (path.HasNextNode(currentNodeIndex))
            {
                return currentNodeIndex + 1;
            }

            direction = Direction.Backward;

            if (path.HasPreviousNode(currentNodeIndex))
            {
                return currentNodeIndex - 1;
            }
        }
        else
        {
            if (path.HasPreviousNode(currentNodeIndex))
            {
                return currentNodeIndex - 1;
            }

            direction = Direction.Forward;

            if (path.HasNextNode(currentNodeIndex))
            {
                return currentNodeIndex + 1;
            }
        }

        return -1;
    }

    public override void ResetPuzzleObject()
    {
        direction = Direction.Forward;
    }
}