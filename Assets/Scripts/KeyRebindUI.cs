using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class KeyRebindUI : MonoBehaviour
{
    public InputActionReference actionReference;
    public int bindingIndex = 0;
    public TMP_Text keyText;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

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
            .OnCancel(operation =>
            {
                operation.Dispose();
                rebindingOperation = null;

                action.Enable();
                UpdateKeyText();
            })
            .OnComplete(operation =>
            {
                operation.Dispose();
                rebindingOperation = null;

                action.Enable();

                UpdateKeyText();

                // Save all binding overrides
                string overrides = action.actionMap.asset
                    .SaveBindingOverridesAsJson();

                PlayerPrefs.SetString("Rebinds", overrides);
                PlayerPrefs.Save();
            });

        rebindingOperation.Start();
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

    private void OnDestroy()
    {
        rebindingOperation?.Dispose();
    }
}