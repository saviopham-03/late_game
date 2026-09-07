using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    private DialogueSet currentDialogue;
    private int currentLineIndex;

    private void Awake()
    {
        Instance = this;
    }

    public void StartDialogue(DialogueSet dialogue)
    {
        currentDialogue = dialogue;
        currentLineIndex = 0;

        ShowCurrentLine();
    }

    private void ShowCurrentLine()
{
    DialogueLine line = currentDialogue.lines[currentLineIndex];

    Debug.Log(line.speaker.name + ": " + line.text);
}

    public void NextLine()
    {
        currentLineIndex++;

        if (currentLineIndex >= currentDialogue.lines.Length)
        {
            EndDialogue();
            return;
        }

        ShowCurrentLine();
    }

    private void EndDialogue()
    {
        currentDialogue = null;
        Debug.Log("Dialogue ended");
    }
}
