using UnityEngine;

public class MenuFloatingAnimation : MonoBehaviour
{
    [SerializeField] private float floatAmount = 10f;
    [SerializeField] private float floatSpeed = 2f;

    private RectTransform rectTransform;
    private Vector2 originalPos;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        originalPos = rectTransform.anchoredPosition;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * floatSpeed) * floatAmount;

        rectTransform.anchoredPosition = originalPos + new Vector2(0f, offset);
    }
}