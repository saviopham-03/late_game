using UnityEngine;
using UnityEngine.EventSystems;

public class SliderSelectionGuard : MonoBehaviour,
    IPointerUpHandler,
    IEndDragHandler
{
    public void OnPointerUp(PointerEventData eventData)
    {
        ClearSelection();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ClearSelection();
    }

    private void OnDisable()
    {
        ClearSelection();
    }

    private void ClearSelection()
    {
        if (EventSystem.current?.currentSelectedGameObject == gameObject)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
