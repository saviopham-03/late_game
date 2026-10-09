using UnityEngine;
using TMPro;
using static System.Math;

public class PopupScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private string popupText;
    [SerializeField] private TMP_FontAsset font;
    [SerializeField] private int fontSize;
    [SerializeField] private float textOffset;
    [SerializeField] private Sprite image;
    [SerializeField] private float imageOffset;
    private float fadeSpeed = 5f;
    private float alpha = 0f;
    private TextMeshPro tmpText;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D collider;
    private GameObject textObj;
    private GameObject imageObj;
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        textObj = new GameObject("FloatingText");
        imageObj = new GameObject("FloatingImage");


        textObj.transform.SetParent(transform);

        textObj.transform.localPosition = collider.offset + Vector2.up * new Vector2(0, collider.size.y/2 + textOffset);

        tmpText = textObj.AddComponent<TextMeshPro>();
        tmpText.text = popupText;
        tmpText.fontSize = fontSize;
        tmpText.font = font;
        tmpText.alignment = TextAlignmentOptions.Center;

        tmpText.sortingOrder = 10; 

        if (image != null)
        {
            imageObj.transform.SetParent(transform);

            imageObj.transform.localPosition =
                collider.offset +
                Vector2.up * (collider.size.y / 2f + imageOffset);

            spriteRenderer =
                imageObj.AddComponent<SpriteRenderer>();

            spriteRenderer.sprite = image;
            spriteRenderer.sortingOrder = 10;
        }

        tmpText.color = new Color(1f,1f,1f,0f);
        spriteRenderer.color = new Color(1f,1f,1f,0f);
    }

    private bool fadingIn = false;

    public void Show()
    {
        fadingIn = true;
    }

    public void Unshow()
    {
        fadingIn = false;
    }

    void OnTriggerStay2D(Collider2D obj)
    {
        if (!obj.gameObject.CompareTag("Player")) return;
        if (!obj.gameObject.GetComponent<PlayerMovement>().IsActive) {
            Unshow();
            return;}
        Show();
    }

    void OnTriggerExit2D(Collider2D obj)
    {
        if (!obj.gameObject.CompareTag("Player")) return;
        if (!obj.gameObject.GetComponent<PlayerMovement>().IsActive) {return;}
        Unshow();
    }

    // Update is called once per frame
    void Update()
    {
        float target = fadingIn ? 1f:0f;
        alpha = Mathf.MoveTowards(alpha, target, fadeSpeed * Time.deltaTime);

        tmpText.color = new Color(1f,1f,1f,alpha);
        spriteRenderer.color = new Color(1f,1f,1f,alpha);
    }
}
