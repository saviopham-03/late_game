using UnityEngine;

public class AreaUnlock : PuzzleOutput
{

    protected override void Activate()
    {
        Debug.Log("active");
        GetComponent<Collider2D>().enabled = false;
    }

    protected override void Deactivate()
    {
        Debug.Log("deactive");
        GetComponent<Collider2D>().enabled = false;
    }
}