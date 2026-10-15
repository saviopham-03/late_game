using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    private float timeScaleBeforeOpen;
    private bool isOpen = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeScaleBeforeOpen = Time.timeScale;
        Unpause();
    }

    // Update is called once per frame
    void Update()
    {
        bool backPressed = Keyboard.current?.escapeKey.wasPressedThisFrame == true;
        if (backPressed) {
            if (isOpen) Unpause();
            else Pause();
        }
    }
    public void ReturnToMainMenu()
    {
        SetPaused(false);
        GameReset.ResetGame();
    }

    public void Pause()
    {
        ShowPanel();
        SetPaused(true);
    }
    public void Unpause()
    {
        UnshowPanel();
        SetPaused(false);
    }


    public void UnshowPanel()
    {
        isOpen = false;
        pausePanel.SetActive(false);
    }
    public void ShowPanel()
    {
        isOpen = true;
        pausePanel.SetActive(true);
    }

    private void SetPaused(bool paused)
    {
        Time.timeScale = paused ? 0f : timeScaleBeforeOpen;
    }
}
