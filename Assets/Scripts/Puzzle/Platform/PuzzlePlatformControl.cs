using UnityEngine;

public class PuzzlePlatformControl : PuzzleOutput
{
    [SerializeField] private Platform platform;

    private void Start()
    {
        ResetPuzzleObject();
    }

    protected override void Activate()
    {
        if (platform == null)
        {
            Debug.LogError(
                "PuzzlePlatformControl has no Platform assigned."
            );
            return;
        }

        platform.SetPuzzleMovementAllowed(true);
    }

    protected override void Deactivate()
    {
        if (platform == null)
        {
            Debug.LogError(
                "PuzzlePlatformControl has no Platform assigned."
            );
            return;
        }

        platform.SetPuzzleMovementAllowed(false);
    }
}