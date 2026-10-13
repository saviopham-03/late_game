using UnityEngine;
using UnityEngine.InputSystem;

public class LevelSelectUI : MonoBehaviour
{
    [SerializeField] private string mainMenuScene = "MainMenu";

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ReturnToMainMenu();
        }
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

    public void ReturnToMainMenu()
    {
        LoadLevel(mainMenuScene);
    }
}