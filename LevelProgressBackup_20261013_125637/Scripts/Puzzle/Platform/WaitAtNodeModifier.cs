using UnityEngine;

[System.Serializable]
public class NodeWait
{
    public int nodeIndex;
    public float waitDuration;
}

public class WaitAtNodeModifier : PlatformModifier
{
    [SerializeField] private NodeWait[] nodeWaits;

    private bool isWaiting = false;
    private float remainingWaitTime = 0f;

    public override void OnNodeReached(int nodeIndex)
    {
        foreach (NodeWait nodeWait in nodeWaits)
        {
            if (nodeWait.nodeIndex == nodeIndex)
            {
                isWaiting = true;
                remainingWaitTime = nodeWait.waitDuration;
                return;
            }
        }
    }

    public override void UpdateModifier()
    {
        if (!isWaiting)
        {
            return;
        }

        remainingWaitTime -= Time.fixedDeltaTime;

        if (remainingWaitTime <= 0f)
        {
            isWaiting = false;
            remainingWaitTime = 0f;
        }
    }

    public override bool BlocksMovement()
    {
        return isWaiting;
    }

    public override void ResetPuzzleObject()
    {
        isWaiting = false;
        remainingWaitTime = 0f;
    }
}