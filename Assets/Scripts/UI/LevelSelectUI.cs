using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class LevelSelectUI : MonoBehaviour
{
    [System.Serializable]
    private class LevelEntry
    {
        public string sceneName;
        public Button button;
        public TMP_Text label;
    }

    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private LevelEntry[] levels = new LevelEntry[3];

    private void Start() => RefreshButtons();

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            ReturnToMainMenu();
    }

    private void RefreshButtons()
    {
        if (levels == null) return;
        for (int i = 0; i < levels.Length && i < LevelProgressManager.TotalLevels; i++)
        {
            var entry = levels[i];
            if (entry == null) continue;
            int levelNumber = i + 1;
            if (entry.button != null) entry.button.interactable = LevelProgressManager.IsUnlocked(levelNumber);
            if (entry.label != null)
            {
                string state = LevelProgressManager.IsCompleted(levelNumber) ? " (Completed)" :
                    LevelProgressManager.IsUnlocked(levelNumber) ? "" : " (Locked)";
                entry.label.text = $"Level {levelNumber}{state}";
            }
        }
    }

    /// <summary>Legacy Unity button binding, still accepts the scene name from Inspector.</summary>
    public void LoadLevel(string sceneName)
    {
        int index = -1;
        if (levels != null)
            for (int i = 0; i < levels.Length; i++)
                if (levels[i] != null && levels[i].sceneName == sceneName) { index = i; break; }
        if (index < 0 || !LevelProgressManager.IsUnlocked(index + 1))
        {
            Debug.LogWarning($"Level '{sceneName}' is locked or not configured.", this);
            return;
        }
        Navigate(sceneName);
    }

    public void LoadLevel1() => LoadConfiguredLevel(1);
    public void LoadLevel2() => LoadConfiguredLevel(2);
    public void LoadLevel3() => LoadConfiguredLevel(3);

    private void LoadConfiguredLevel(int level)
    {
        if (!LevelProgressManager.IsUnlocked(level) || levels == null || levels.Length < level ||
            levels[level - 1] == null) return;
        LoadLevel(levels[level - 1].sceneName);
    }

    public void ReturnToMainMenu() => Navigate(mainMenuScene);

    private void Navigate(string sceneName)
    {
        if (SceneNavigationManager.Instance == null)
        {
            Debug.LogError("SceneNavigationManager is missing.", this);
            return;
        }
        SceneNavigationManager.Instance.LoadScene(sceneName);
    }

    [ContextMenu("Reset Saved Level Progress (Testing)")]
    private void ResetSavedProgress()
    {
        LevelProgressManager.ResetProgress();
        RefreshButtons();
    }
}
