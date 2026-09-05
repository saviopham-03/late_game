using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField]
    private string firstLevelScene = "PuzzleObjectTest";

    public void StartGame()
    {
        if (SceneNavigationManager.Instance == null)
        {
            Debug.LogError("SceneNavigationManager is missing.");
            return;
        }

        SceneNavigationManager.Instance.LoadScene(firstLevelScene);
    }
}