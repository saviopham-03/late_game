using UnityEngine;
using UnityEngine.UI;

public class ImageDebug : MonoBehaviour
{
    [SerializeField] private Image targetImage;

    private void Start()
    {
        if (targetImage == null)
        {
            Debug.LogError("Target image is not assigned!");
            return;
        }

        // Force the image to be visible.
        targetImage.enabled = true;
        targetImage.gameObject.SetActive(true);
        targetImage.color = Color.white;

        RectTransform rect = targetImage.rectTransform;
        rect.localScale = Vector3.one;

        Debug.Log($"Sprite: {targetImage.sprite}");
        Debug.Log($"Position: {rect.anchoredPosition}");
        Debug.Log($"Size: {rect.rect.size}");
        Debug.Log($"Color: {targetImage.color}");
        Debug.Log($"Material: {targetImage.material}");
    }
}