using UnityEngine;
using System.Collections.Generic;


public class NPCDialogue : MonoBehaviour
{

    [SerializeField] private bool forceDialogue;

    [SerializeField] public DialogueSet dialogue;
    private List<PlayerMovement> closeClones = new List<PlayerMovement>();


    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Player")) {
            DialogueManager.Instance.AddInRangeNPC(this);
            closeClones.Add(other.GetComponent<PlayerMovement>());
        }
        if (forceDialogue)
        {
            forceDialogue = false;
            DialogueManager.Instance.ForceDialogue(dialogue, NearActiveClone());
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) {
            DialogueManager.Instance.RemoveNPC(this);
            closeClones.Remove(other.GetComponent<PlayerMovement>());
        }
    }
    public GameObject NearActiveClone()
    {
        foreach (PlayerMovement clone in closeClones)
        {
            if (clone.IsActive) return clone.gameObject;
        }
        return null;
    }
}