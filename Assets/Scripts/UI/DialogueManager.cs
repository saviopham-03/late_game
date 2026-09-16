using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;


public class DialogueManager : MonoBehaviour
{
    public Transform player;
    public static DialogueManager Instance;
    private DialogueSet currentDialogue;
    private int currentLineIndex;
    private List<NPCDialogue> nearbyNPCs = new List<NPCDialogue>();
    private NPCDialogue activeDialogue;

    [SerializeField] InputActionReference interactAction;

    private void Awake()
    {
        Instance = this;
        interactAction.action.Enable();
        interactAction.action.started += _ =>
        {
            if (currentDialogue == null) StartDialogue();
            else NextLine();
        };
    }

    public void AddInRangeNPC(NPCDialogue npc)
    {
        if (!nearbyNPCs.Contains(npc))
        {
            nearbyNPCs.Add(npc);
        }
    }

    public void RemoveNPC(NPCDialogue npc)
    {
        nearbyNPCs.Remove(npc);
    }

    public NPCDialogue GetClosestNPC()
    {
        NPCDialogue closest = null;
        float closestDistanceSqr = Mathf.Infinity;

        foreach (NPCDialogue npc in nearbyNPCs)
        {
            if (npc == null)
                continue;

            float distanceSqr = (npc.transform.position - player.position).sqrMagnitude;

            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                closest = npc;
            }
        }

        return closest;
    }

    public void StartDialogue()
    {
        if (currentDialogue == null)
        {
            NPCDialogue dialogue_npc = GetClosestNPC();
            currentDialogue = dialogue_npc.dialogue;
            currentLineIndex = 0;
            ShowCurrentLine();
        }
    }

    private void ShowCurrentLine()
{
    DialogueLine line = currentDialogue.lines[currentLineIndex];

    Debug.Log(line.speaker.name + ": " + line.text);
}

    public void NextLine()
    {
        if (currentDialogue != null)
        {
            currentLineIndex++;

            if (currentLineIndex >= currentDialogue.lines.Length)
            {
                EndDialogue();
                return;
            }

            ShowCurrentLine();
        }
    }

    private void EndDialogue()
    {
        currentDialogue = null;
        Debug.Log("Dialogue ended");
    }
}
