using UnityEngine;

public class Path : PuzzleObject
{
    [SerializeField] private Transform[] nodes;
    [SerializeField] private int startNodeIndex = 0;

    public Vector3 GetNodePosition(int index)
    {
        if (index < 0 || index >= nodes.Length)
        {
            Debug.LogWarning($"Path node index {index} is out of range.");
            return Vector3.zero;
        }

        return nodes[index].position;
    }

    public int GetNodeCount()
    {
        return nodes.Length;
    }

    public int GetStartNodeIndex()
    {
        return startNodeIndex;
    }

    public int GetFirstNodeIndex()
    {
        return 0;
    }

    public int GetLastNodeIndex()
    {
        return nodes.Length - 1;
    }

    public bool HasNextNode(int index)
    {
        return index < GetLastNodeIndex();
    }

    public bool HasPreviousNode(int index)
    {
        return index > GetFirstNodeIndex();
    }

    public override void ResetPuzzleObject()
    {
        // Path stores node positions only.
        // It has no traversal state that needs to be reset.
    }
}