using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonSounds : MonoBehaviour, IPointerEnterHandler
{
    public UISoundManager uiSoundManager;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        if (button != null && uiSoundManager != null)
        {
            button.onClick.AddListener(uiSoundManager.PlayClick);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (uiSoundManager != null)
        {
            uiSoundManager.PlayHover();
        }
    }

    private void OnDestroy()
    {
        if (button != null && uiSoundManager != null)
        {
            button.onClick.RemoveListener(uiSoundManager.PlayClick);
        }
    }
}