using UnityEngine;
using TMPro;

public class PopupScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private string popupText;
    [SerializeField] private TMP_FontAsset font;
    [SerializeField] private int fontSize;
    [SerializeField] private float textOffset;

    private TextMeshPro tmpText;
    private BoxCollider2D collider;
    private GameObject textObj;
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        textObj = new GameObject("FloatingText");
        textObj.transform.SetParent(transform);

        textObj.transform.localPosition = collider.offset + Vector2.up * new Vector2(0, collider.bounds.extents.y + textOffset);

        tmpText = textObj.AddComponent<TextMeshPro>();
        tmpText.text = popupText;
        tmpText.fontSize = fontSize;
        tmpText.font = font;
        tmpText.alignment = TextAlignmentOptions.Center;

        tmpText.sortingOrder = 10; 

        textObj.SetActive(false);
    }

    void OnTriggerStay2D(Collider2D obj)
    {
        if (!obj.gameObject.CompareTag("Player")) return;
        if (!obj.gameObject.GetComponent<PlayerMovement>().IsActive) {textObj.SetActive(false);return;}
        textObj.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D obj)
    {
        if (!obj.gameObject.CompareTag("Player")) return;
        if (!obj.gameObject.GetComponent<PlayerMovement>().IsActive) {return;}
        textObj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
