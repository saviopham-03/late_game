using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [SerializeField] float interactRadius = 10;

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