using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class LevelProgressTests
{
    [SetUp] public void SetUp() => LevelProgressManager.ResetProgress();
    [TearDown] public void TearDown() => LevelProgressManager.ResetProgress();

    [Test] public void FreshSave_OnlyFirstLevelUnlocked()
    {
        Assert.AreEqual(1, LevelProgressManager.HighestUnlockedLevel);
        Assert.IsTrue(LevelProgressManager.IsUnlocked(1));
        Assert.IsFalse(LevelProgressManager.IsUnlocked(2));
        Assert.IsFalse(LevelProgressManager.IsUnlocked(3));
    }

    [TestCase(0)] [TestCase(-1)] [TestCase(4)]
    public void InvalidLevel_IsNeitherUnlockedNorCompleted(int level)
    {
        Assert.IsFalse(LevelProgressManager.IsUnlocked(level));
        Assert.IsFalse(LevelProgressManager.IsCompleted(level));
    }

    [Test] public void FirstCompletion_UnlocksSecondAndMarksFirstCompleted()
    {
        LevelProgressManager.CompleteLevel(1);
        Assert.IsTrue(LevelProgressManager.IsCompleted(1));
        Assert.IsTrue(LevelProgressManager.IsUnlocked(2));
        Assert.IsFalse(LevelProgressManager.IsUnlocked(3));
    }

    [Test] public void LockedLevelCannotBeCompletedOutOfOrder()
    {
        LogAssert.Expect(LogType.Warning, "Cannot complete locked/invalid level 3.");
        LevelProgressManager.CompleteLevel(3);
        Assert.IsFalse(LevelProgressManager.IsCompleted(3));
        Assert.AreEqual(1, LevelProgressManager.HighestUnlockedLevel);
    }

    [Test] public void SecondCompletion_UnlocksThird()
    {
        LevelProgressManager.CompleteLevel(1);
        LevelProgressManager.CompleteLevel(2);
        Assert.IsTrue(LevelProgressManager.IsCompleted(2));
        Assert.IsTrue(LevelProgressManager.IsUnlocked(3));
        Assert.IsFalse(LevelProgressManager.IsCompleted(3));
    }

    [Test] public void FinalCompletion_MarksThirdCompletedWithoutUnlockingFourth()
    {
        LevelProgressManager.CompleteLevel(1);
        LevelProgressManager.CompleteLevel(2);
        LevelProgressManager.CompleteLevel(3);
        Assert.IsTrue(LevelProgressManager.IsCompleted(3));
        Assert.IsFalse(LevelProgressManager.IsUnlocked(4));
    }

    [Test] public void CompletingEarlierLevelAgain_DoesNotReduceProgress()
    {
        LevelProgressManager.CompleteLevel(1);
        LevelProgressManager.CompleteLevel(2);
        LevelProgressManager.CompleteLevel(1);
        Assert.AreEqual(3, LevelProgressManager.HighestUnlockedLevel);
    }

    [Test] public void ResetProgress_ClearsCompletionAndUnlocks()
    {
        LevelProgressManager.CompleteLevel(1);
        LevelProgressManager.CompleteLevel(2);
        LevelProgressManager.CompleteLevel(3);
        LevelProgressManager.ResetProgress();
        Assert.AreEqual(1, LevelProgressManager.HighestUnlockedLevel);
        Assert.IsFalse(LevelProgressManager.IsCompleted(1));
        Assert.IsFalse(LevelProgressManager.IsCompleted(3));
    }
}
