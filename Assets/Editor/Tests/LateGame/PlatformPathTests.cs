using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlatformPathTests
{
    private GameObject go;
    private Path path;
    private static void SetPrivate(object obj, string name, object value)
        => typeof(Path).GetField(name, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(obj, value);

    [SetUp] public void SetUp()
    {
        go = new GameObject("test-path");
        path = go.AddComponent<Path>();
        var nodes = new Transform[3];
        for (int i=0;i<3;i++)
        {
            var n = new GameObject("node"+i);
            n.transform.parent = go.transform;
            n.transform.position = new Vector3(i * 2, i, 0);
            nodes[i] = n.transform;
        }
        SetPrivate(path, "nodes", nodes);
        SetPrivate(path, "startNodeIndex", 1);
    }
    [TearDown] public void TearDown() => Object.DestroyImmediate(go);

    [Test] public void Path_GettersReturnConfiguredNodesAndIndices()
    {
        Assert.AreEqual(3, path.GetNodeCount());
        Assert.AreEqual(1, path.GetStartNodeIndex());
        Assert.AreEqual(0, path.GetFirstNodeIndex());
        Assert.AreEqual(2, path.GetLastNodeIndex());
        Assert.AreEqual(new Vector3(4,2,0), path.GetNodePosition(2));
    }

    [Test] public void Path_NextPreviousBoundaryChecks()
    {
        Assert.IsTrue(path.HasNextNode(0));
        Assert.IsFalse(path.HasNextNode(2));
        Assert.IsFalse(path.HasPreviousNode(0));
        Assert.IsTrue(path.HasPreviousNode(2));
    }

    [Test] public void Path_InvalidNodeReturnsZeroAndWarns()
    {
        LogAssert.Expect(LogType.Warning, "Path node index 5 is out of range.");
        Assert.AreEqual(Vector3.zero, path.GetNodePosition(5));
    }

    [Test] public void Loop_WrapsAtEnd()
    {
        var loop = go.AddComponent<Loop>();
        Assert.AreEqual(1, loop.GetNextNodeIndex(0, path));
        Assert.AreEqual(0, loop.GetNextNodeIndex(2, path));
    }

    [Test] public void StopAtEnd_ReturnsSentinelAtEnd()
    {
        var stop = go.AddComponent<StopAtEnd>();
        Assert.AreEqual(2, stop.GetNextNodeIndex(1, path));
        Assert.AreEqual(-1, stop.GetNextNodeIndex(2, path));
    }

    [Test] public void PingPong_ReversesAtEndsAndResetsDirection()
    {
        var ping = go.AddComponent<PingPong>();
        Assert.AreEqual(1, ping.GetNextNodeIndex(0,path));
        Assert.AreEqual(2, ping.GetNextNodeIndex(1,path));
        Assert.AreEqual(1, ping.GetNextNodeIndex(2,path));
        Assert.AreEqual(0, ping.GetNextNodeIndex(1,path));
        Assert.AreEqual(1, ping.GetNextNodeIndex(0,path));
        ping.ResetPuzzleObject();
        Assert.AreEqual(1, ping.GetNextNodeIndex(0,path));
    }

    [Test] public void ContinuousMovement_AlwaysWantsToMove()
    {
        Assert.IsTrue(go.AddComponent<ContinuousMovement>().ShouldMove());
    }
}
