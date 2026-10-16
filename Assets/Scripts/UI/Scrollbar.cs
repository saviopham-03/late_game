using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Scrollbar : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform handle;
    [SerializeField, Range(0f, 1f)] private float handleSize = 0.2f;

    private RectTransform track;
    private float dragOffset;

    void Awake()
    {
        track = GetComponent<RectTransform>();

        if (handle != null)
        {
            handle.anchorMin = new Vector2(0f, 1f);
            handle.anchorMax = new Vector2(1f, 1f);
            handle.pivot = new Vector2(0.5f, 1f);

            handle.sizeDelta = new Vector2(0f, -track.rect.height * (1f - handleSize));
        }
    }

    void LateUpdate()
    {
        if (scrollRect == null || handle == null)
            return;

        float normalizedPosition = scrollRect.verticalNormalizedPosition;
        float availableHeight = track.rect.height - handle.rect.height;

        Vector2 position = handle.anchoredPosition;
        position.y = -availableHeight * (1f - normalizedPosition);
        handle.anchoredPosition = position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            track, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);

        dragOffset = handle.anchoredPosition.y - localPoint.y;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (scrollRect == null || handle == null)
            return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            track, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);

        float availableHeight = track.rect.height - handle.rect.height;

        if (availableHeight <= 0f)
            return;

        float y = Mathf.Clamp(localPoint.y + dragOffset, -availableHeight, 0f);
        float normalizedPosition = 1f - (-y / availableHeight);

        scrollRect.verticalNormalizedPosition = normalizedPosition;
    }
}