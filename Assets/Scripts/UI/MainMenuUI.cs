using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private string firstLevelScene = "PuzzleObjectTest";
    [SerializeField] private string levelSelectScene = "LevelSelect";

    public void StartGame()
    {
        LoadLevel(firstLevelScene);
    }

    public void OpenLevelSelect()
    {
        LoadLevel(levelSelectScene);
    }

    public void LoadLevel(string sceneName)
    {
        if (SceneNavigationManager.Instance == null)
        {
            Debug.LogError("SceneNavigationManager is missing.", this);
            return;
        }

        SceneNavigationManager.Instance.LoadScene(sceneName);
    }
}