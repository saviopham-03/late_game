using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class KeyRebindUI : MonoBehaviour
{
    public InputActionReference actionReference;
    public int bindingIndex = 0;
    public TMP_Text keyText;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    private bool wasActionEnabled;
    private int lastCancelledFrame = -1;

    public bool IsRebinding => rebindingOperation != null;
    public bool CancelledThisFrame => lastCancelledFrame == Time.frameCount;

    private void Start()
    {
        // Load saved key bindings when the game starts
        if (PlayerPrefs.HasKey("Rebinds"))
        {
            string overrides = PlayerPrefs.GetString("Rebinds");

            if (!string.IsNullOrEmpty(overrides) && actionReference != null)
            {
                actionReference.action.actionMap.asset
                    .LoadBindingOverridesFromJson(overrides);
            }
        }

        UpdateKeyText();
    }

    public void StartRebind()
    {
        if (actionReference == null)
        {
            Debug.LogWarning("Action Reference has not been assigned.");
            return;
        }

        InputAction action = actionReference.action;

        if (bindingIndex < 0 || bindingIndex >= action.bindings.Count)
        {
            Debug.LogWarning("Invalid binding index for " + action.name);
            return;
        }

        CancelRebind();

        wasActionEnabled = action.enabled;
        action.Disable();

        if (keyText != null)
        {
            keyText.text = "Press a key...";
        }

        // Dispose any previous rebinding operation
        rebindingOperation?.Dispose();

        rebindingOperation = action
            .PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("Mouse")
            .WithCancelingThrough("<Keyboard>/escape")
            .OnCancel(operation =>
            {
                operation.Dispose();
                rebindingOperation = null;
                lastCancelledFrame = Time.frameCount;
                RestoreActionState(action);
                UpdateKeyText();
            })
            .OnComplete(operation =>
            {
                operation.Dispose();
                rebindingOperation = null;

                RestoreActionState(action);

                UpdateKeyText();

                // Save all binding overrides
                string overrides = action.actionMap.asset
                    .SaveBindingOverridesAsJson();

                PlayerPrefs.SetString("Rebinds", overrides);
                PlayerPrefs.Save();
            });

        rebindingOperation.Start();
    }

    public void CancelRebind()
    {
        rebindingOperation?.Cancel();
    }

    private void UpdateKeyText()
    {
        if (actionReference == null || keyText == null)
        {
            return;
        }

        InputAction action = actionReference.action;

        if (bindingIndex >= 0 && bindingIndex < action.bindings.Count)
        {
            keyText.text = action.GetBindingDisplayString(bindingIndex);
        }
    }

    private void RestoreActionState(InputAction action)
    {
        if (wasActionEnabled)
        {
            action.Enable();
        }
    }

    private void OnDisable()
    {
        CancelRebind();
    }

    private void OnDestroy()
    {
        CancelRebind();
    }
}
