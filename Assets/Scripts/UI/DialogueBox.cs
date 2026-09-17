using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DialogueBox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private TextMeshProUGUI nameComponent;
    [SerializeField] private Image imgComponent;
    [SerializeField] private PlayerMovement player;
    public float textSpeed;
    private string line;
    private bool active = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        GetComponent<Image>().color = new Color(1f,1f,1f,0f);
        imgComponent.color = new Color(1f,1f,1f,0f);
        textComponent.text = string.Empty;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void showBox()
    {
        GetComponent<Image>().color = new Color(1f,1f,1f,1f);
        imgComponent.color = new Color(1f,1f,1f,1f);
    }

    public int showLine(DialogueLine d_line)
    {

        if (!active) showBox();
        else
        {
            if (textComponent.text != line)
            {
                StopAllCoroutines();
                textComponent.text = line;
                return -1;
            }
        }
        if (d_line==null)
        {
            endLine();
            return 0;
        }
        string c_line = d_line.text;
        active = true;
        textComponent.text = string.Empty;
        nameComponent.text = d_line.speaker.name;
        imgComponent.sprite = d_line.speaker.closeup;
        line=c_line;
        StartCoroutine(TypeLine());
        return 0;
    }

    private void endLine()
    {
        textComponent.text = string.Empty;
        nameComponent.text = string.Empty;
        imgComponent.sprite = null;
        imgComponent.color = new Color(1f,1f,1f,0f);
        GetComponent<Image>().color = new Color(1f,1f,1f,0f);
        active = false;
    }

    IEnumerator TypeLine()
    {
        foreach (char c in line.ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }
}
