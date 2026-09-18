using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CloneList
{
    public GameObject Value;
    public CloneList Next;
    public CloneList Previous;

}

public class CloneManager : MonoBehaviour
{
    public static CloneManager Instance;
    [SerializeField] private InputActionReference switchAction;

    [SerializeField] GameObject player;

    private Dictionary<BoxCollider2D, CloneList> cloneSets = new();
    private CloneList current = new();
    private BoxCollider2D currentSpace;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        current.Value = player;
        current.Next = current;
        current.Previous = current;
        switchAction.action.Enable();
    }

    public void Disable() {
        switchAction.action.Disable();
    }
    public void Enable() {
        switchAction.action.Enable();
    }

    public void switchCloneSet(BoxCollider2D newSpace)
    {
        // Debug.Log("Entered space: " + newSpace.GetInstanceID());
        if (!cloneSets.ContainsKey(newSpace))
        {
            removeFromCurrentSet(current);
            cloneSets.Add(newSpace, current);
            current.Next = current;
            current.Previous = current;
        }
        else
        {
            CloneList current_new = cloneSets[newSpace];

            removeFromCurrentSet(current);
            current_new.Next.Previous = current;
            current.Next = current_new.Next;
            current_new.Next = current;
            current.Previous = current_new;
        }
        currentSpace = newSpace;
    }

    private void removeFromCurrentSet(CloneList clone)
    {
        if (currentSpace == null) return;

        if (clone.Next == clone) // only one
        {
            cloneSets.Remove(currentSpace);
            return;
        }
        cloneSets[currentSpace] = clone.Previous;
        clone.Previous.Next = clone.Next;
        clone.Next.Previous = clone.Previous;
    }

    public void addClone(GameObject newClone)
    {
        PlayerMovement clone_movement = newClone.GetComponent<PlayerMovement>();
        clone_movement.setActive(false);
        CloneList new_node = new()
        {
            Value = newClone,
            Previous = current,
            Next = current.Next
        };
        current.Next.Previous = new_node;
        current.Next = new_node;
    }

    private void switchClone()
    {
        PlayerMovement current_player = current.Value.GetComponent<PlayerMovement>();
        current_player.setActive(false);

        PlayerMovement next = current.Next.Value.GetComponent<PlayerMovement>();
        next.setActive(true);
        current = current.Next;
    }

    void Update()
    {
        switchAction.action.started += ctx =>
        {
            switchClone();
        };
    }
}
