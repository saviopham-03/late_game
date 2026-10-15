using UnityEngine;

public class AreaUnlock : PuzzleOutput
{
    [SerializeField] private bool invert = false;
    void Start()
    {
        Deactivate();
    }

    protected override void Activate()
    {
        GetComponent<Collider2D>().enabled = invert;
    }

    protected override void Deactivate()
    {
        GetComponent<Collider2D>().enabled = !invert;
    }
}