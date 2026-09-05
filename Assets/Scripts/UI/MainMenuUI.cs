using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField]
    private string firstLevelScene = "PuzzleObjectTest";

    [Header("Main menu")]
    [SerializeField] private GameObject gameTitle;
    [SerializeField] private Button startButton;
    [SerializeField] private Button levelSelectButton;

    [Header("Level select")]
    [SerializeField] private GameObject levelSelectPanel;

    private void Start()
    {
        if (SetLevelSelectVisible(false))
            SelectButton(startButton);
    }

    private void Update()
    {
        if (levelSelectPanel != null && levelSelectPanel.activeInHierarchy &&
            Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseLevelSelect();
        }
    }

    public void StartGame()
    {
        LoadLevel(firstLevelScene);
    }

    public void OpenLevelSelect()
    {
        if (SetLevelSelectVisible(true))
            SelectButton(levelSelectPanel.GetComponentInChildren<Button>());
    }

    public void CloseLevelSelect()
    {
        if (SetLevelSelectVisible(false))
            SelectButton(levelSelectButton);
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

    private bool SetLevelSelectVisible(bool visible)
    {
        if (gameTitle == null || startButton == null ||
            levelSelectButton == null || levelSelectPanel == null)
        {
            Debug.LogError(
                "Assign Game Title, Start Button, Level Select Button and " +
                "Level Select Panel on Canvas > MainMenuUI.", this);
            return false;
        }

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        gameTitle.SetActive(!visible);
        startButton.gameObject.SetActive(!visible);
        levelSelectButton.gameObject.SetActive(!visible);
        levelSelectPanel.SetActive(visible);
        return true;
    }

    private static void SelectButton(Button button)
    {
        if (EventSystem.current != null && button != null &&
            button.gameObject.activeInHierarchy && button.IsInteractable())
        {
            EventSystem.current.SetSelectedGameObject(button.gameObject);
        }
    }
}