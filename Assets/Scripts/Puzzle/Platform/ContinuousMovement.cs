public class ContinuousMovement : MovementBehaviour
{
    public override bool ShouldMove()
    {
        return true;
    }

    public override void ResetPuzzleObject()
    {
        // No internal state to reset.
    }
}