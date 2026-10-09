using UnityEngine;

public class AreaUnlock : PuzzleOutput
{

    protected override void Activate()
    {
        GetComponent<Collider2D>().enabled = false;
    }

    protected override void Deactivate()
    {
        GetComponent<Collider2D>().enabled = false;
    }
}