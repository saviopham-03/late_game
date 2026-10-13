using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DisallowMultipleComponent]
[DefaultExecutionOrder(-100)]
public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private string mainMenuScene = "MainMenu";

    private readonly List<InputAction> pausedActions = new List<InputAction>();
    private bool isPaused;
    private float previousTimeScale;
    private bool previousAudioPause;
    private CursorLockMode previousCursorLock;
    private bool previousCursorVisible;

    private void Awake()
    {
        if (pausePanel == null || resumeButton == null ||
            restartButton == null || mainMenuButton == null)
        {
            Debug.LogError("Assign the pause panel and all three buttons in PauseMenuUI.", this);
            enabled = false;
            return;
        }

        if (pausePanel == gameObject || transform.IsChildOf(pausePanel.transform))
        {
            Debug.LogError("Attach PauseMenuUI to PauseCanvas, outside PausePanel.", this);
            enabled = false;
            return;
        }

        pausePanel.SetActive(false);
        SetNavigation(resumeButton, mainMenuButton, restartButton);
        SetNavigation(restartButton, resumeButton, mainMenuButton);
        SetNavigation(mainMenuButton, restartButton, resumeButton);
    }

    private void OnEnable()
    {
        resumeButton.onClick.AddListener(ResumeGame);
        restartButton.onClick.AddListener(RestartLevel);
        mainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    private void OnDisable()
    {
        ResumeGame();
        if (resumeButton != null) resumeButton.onClick.RemoveListener(ResumeGame);
        if (restartButton != null) restartButton.onClick.RemoveListener(RestartLevel);
        if (mainMenuButton != null) mainMenuButton.onClick.RemoveListener(ReturnToMainMenu);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        if (isPaused || !isActiveAndEnabled) return;

        previousTimeScale = Time.timeScale;
        previousAudioPause = AudioListener.pause;
        previousCursorLock = Cursor.lockState;
        previousCursorVisible = Cursor.visible;
        isPaused = true;

        Time.timeScale = 0f;
        AudioListener.pause = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Pause this project's gameplay actions while keeping the UI map enabled.
        pausedActions.Clear();
        foreach (InputAction action in InputSystem.ListEnabledActions())
        {
            string mapName = action.actionMap != null ? action.actionMap.name : "";
            if (mapName != "Gameplay" && mapName != "Player") continue;

            pausedActions.Add(action);
            action.Disable();
        }

        pausePanel.SetActive(true);
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
        }
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;
        Time.timeScale = previousTimeScale;
        AudioListener.pause = previousAudioPause;
        Cursor.lockState = previousCursorLock;
        Cursor.visible = previousCursorVisible;

        if (pausePanel != null) pausePanel.SetActive(false);
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);

        // Restore only actions that were enabled before this menu opened.
        foreach (InputAction action in pausedActions) action.Enable();
        pausedActions.Clear();
    }

    public void RestartLevel()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        LoadScene(mainMenuScene);
    }

    private void LoadScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName) ||
            !Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError("Add the scene to Build Profiles > Scene List: " + sceneName, this);
            return;
        }

        ResumeGame();
        if (SceneNavigationManager.Instance != null)
            SceneNavigationManager.Instance.LoadScene(sceneName);
        else
            SceneManager.LoadScene(sceneName);
    }

    private static void SetNavigation(Button button, Button up, Button down)
    {
        Navigation navigation = new Navigation
        {
            mode = Navigation.Mode.Explicit,
            selectOnUp = up,
            selectOnDown = down
        };
        button.navigation = navigation;
    }
}
