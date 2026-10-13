using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class OptionsMenuController : MonoBehaviour
{
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject previousMenu;
    [SerializeField] private bool pauseWhileOpen = true;

    private KeyRebindUI[] rebindControls;
    private float timeScaleBeforeOpen = 1f;
    private bool returnToPreviousMenu;

    public bool IsOpen => optionsPanel != null && optionsPanel.activeSelf;

    private void Awake()
    {
        if (optionsPanel == null)
        {
            Debug.LogError("Options panel has not been assigned.", this);
            enabled = false;
            return;
        }

        rebindControls = optionsPanel.GetComponentsInChildren<KeyRebindUI>(true);

        if (IsOpen)
        {
            timeScaleBeforeOpen = Time.timeScale;
            SetPaused(true);
        }
    }

    private void Update()
    {
        bool backPressed =
            Keyboard.current?.escapeKey.wasPressedThisFrame == true ||
            Gamepad.current?.buttonEast.wasPressedThisFrame == true;

        if (!backPressed)
        {
            return;
        }

        HandleBack();
    }

    public void HandleBack()
    {
        if (CancelActiveRebind())
        {
            return;
        }

        ToggleOptions();
    }

    public void ToggleOptions()
    {
        if (IsOpen)
        {
            HideOptions();
        }
        else
        {
            ShowOptions();
        }
    }

    public void ShowOptions()
    {
        if (IsOpen)
        {
            return;
        }

        timeScaleBeforeOpen = Time.timeScale;
        returnToPreviousMenu = previousMenu != null && previousMenu.activeSelf;

        if (returnToPreviousMenu)
        {
            previousMenu.SetActive(false);
        }

        optionsPanel.SetActive(true);
        SetPaused(true);
        ClearSelection();
    }

    public void HideOptions()
    {
        if (!IsOpen)
        {
            return;
        }

        CancelActiveRebind();
        optionsPanel.SetActive(false);

        if (returnToPreviousMenu && previousMenu != null)
        {
            previousMenu.SetActive(true);
        }
        else
        {
            SetPaused(false);
        }

        returnToPreviousMenu = false;
        ClearSelection();
    }

    private bool CancelActiveRebind()
    {
        bool consumedBackInput = false;

        foreach (KeyRebindUI rebindControl in rebindControls)
        {
            if (rebindControl.IsRebinding)
            {
                rebindControl.CancelRebind();
                consumedBackInput = true;
            }
            else if (rebindControl.CancelledThisFrame)
            {
                consumedBackInput = true;
            }
        }

        return consumedBackInput;
    }

    private void SetPaused(bool paused)
    {
        if (!pauseWhileOpen)
        {
            return;
        }

        Time.timeScale = paused ? 0f : timeScaleBeforeOpen;
    }

    private static void ClearSelection()
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private void OnDestroy()
    {
        if (IsOpen)
        {
            SetPaused(false);
        }
    }
}
