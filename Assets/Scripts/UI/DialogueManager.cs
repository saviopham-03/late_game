using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;


public class DialogueManager : MonoBehaviour
{
    private GameObject active_player;
    public static DialogueManager Instance;
    private DialogueSet currentDialogue;
    private int currentLineIndex;
    private List<NPCDialogue> nearbyNPCs = new List<NPCDialogue>();
    private NPCDialogue activeDialogue;

    [SerializeField] InputActionReference interactAction;
    [SerializeField] DialogueBox _dialogueBox;

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
            if (active_player == null) active_player = npc.NearActiveClone();
            if (npc == null || active_player==null)
                continue;

            float distanceSqr = (npc.transform.position - active_player.transform.position).sqrMagnitude;

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

            active_player.GetComponent<PlayerMovement>().DisableMovement();
            CloneManager.Instance.Disable();

            currentDialogue = dialogue_npc.dialogue;
            currentLineIndex = 0;
            ShowCurrentLine();
        }
    }

    private void ShowCurrentLine()
{
    DialogueLine line = currentDialogue.lines[currentLineIndex];

    int ret = _dialogueBox.showLine(line);
    if (ret == -1) currentLineIndex--; // last text wasn't finished rendering
    // Debug.Log(line.speaker.name + ": " + line.text);
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
        int ret = _dialogueBox.showLine(null);
        if (ret == -1)
        {
            currentLineIndex--;
            return;
        }
        currentDialogue = null;
        active_player.GetComponent<PlayerMovement>().EnableMovement();
        CloneManager.Instance.Enable();
        // Debug.Log("Dialogue ended");
    }
}
