using UnityEngine;

public class NPCDialogue : MonoBehaviour
{

    public DialogueSet dialogue;
    public bool interactable;

    private void Awake()
    {
        interactable = false;
    }

    private void OnTriggerEnter2D()
    {
        DialogueManager.Instance.AddInRangeNPC(this);
    }
    private void OnTriggerExit2D()
    {
        DialogueManager.Instance.RemoveNPC(this);
    }
}