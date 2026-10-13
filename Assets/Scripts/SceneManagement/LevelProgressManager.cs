using UnityEngine;

/// <summary>Persistent linear three-level progression. Level one is always unlocked.</summary>
public static class LevelProgressManager
{
    private const string HighestUnlockedKey = "LateGame.HighestUnlockedLevel.v1";
    private const string FinalCompletedKey = "LateGame.Level3Completed.v1";
    public const int TotalLevels = 3;

    public static int HighestUnlockedLevel => Mathf.Clamp(PlayerPrefs.GetInt(HighestUnlockedKey, 1), 1, TotalLevels);
    public static bool IsUnlocked(int level) => level >= 1 && level <= TotalLevels && level <= HighestUnlockedLevel;
    public static bool IsCompleted(int level) => level >= 1 && level <= TotalLevels && (level < HighestUnlockedLevel || (level == TotalLevels && PlayerPrefs.GetInt(FinalCompletedKey, 0) == 1));

    public static void CompleteLevel(int level)
    {
        if (!IsUnlocked(level))
        {
            Debug.LogWarning($"Cannot complete locked/invalid level {level}.");
            return;
        }
        if (level == TotalLevels)
        {
            PlayerPrefs.SetInt(FinalCompletedKey, 1);
            PlayerPrefs.Save();
        }
        int next = Mathf.Min(level + 1, TotalLevels);
        if (next > HighestUnlockedLevel)
        {
            PlayerPrefs.SetInt(HighestUnlockedKey, next);
            PlayerPrefs.Save();
        }
    }

    // Testing only: invoke from the component's context menu in the Unity Inspector.
    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(HighestUnlockedKey);
        PlayerPrefs.DeleteKey(FinalCompletedKey);
        PlayerPrefs.Save();
    }
}
