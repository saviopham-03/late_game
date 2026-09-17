using UnityEngine;
using System.Collections.Generic;


public class NPCDialogue : MonoBehaviour
{

    public DialogueSet dialogue;
    public bool interactable;
    private List<PlayerMovement> closeClones = new List<PlayerMovement>();

    private void Awake()
    {
        interactable = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) {
            DialogueManager.Instance.AddInRangeNPC(this);
            closeClones.Add(other.GetComponent<PlayerMovement>());
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