using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    public DialogueSet dialogue;

    public void Interact()
    {
        DialogueManager.Instance.StartDialogue(dialogue);
    }

    private void OnCollisionEnter2D()
    {
        Interact();
    }
}