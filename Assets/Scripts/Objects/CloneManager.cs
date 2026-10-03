using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CloneList
{
    public GameObject Value;
    public CloneList Next;
    public CloneList Previous;
    public BoxCollider2D Space;

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

    public void AssignStartingSpace(BoxCollider2D newSpace)
    {
        current.Space = newSpace;
    }

    public void Disable() {
        switchAction.action.Disable();
    }
    public void Enable() {
        switchAction.action.Enable();
    }

    private CloneList findByGameObj(GameObject clone)
    {
        foreach (CloneList start in cloneSets.Values)
        {
            CloneList search = start;

            do
            {
                if (search.Value == clone)
                    return search;

                search = search.Next;
            }
            while (search != start);
        }

        return null;
    }

    public void switchCloneSet(BoxCollider2D newSpace, GameObject clone=null)
    {
        // Debug.Log("Entered space: " + newSpace.GetInstanceID());
        CloneList new_clone_list = (clone == null) ? current:findByGameObj(clone);

        if (new_clone_list == null) // clone doesn't have an existing clonelist element so make one
        {
            new_clone_list = new();
            new_clone_list.Next = new_clone_list;
            new_clone_list.Value = clone;
            new_clone_list.Previous = new_clone_list;
            new_clone_list.Space = newSpace;
        }

        removeFromCurrentSet(new_clone_list);
        new_clone_list.Space = newSpace;
        
        if (!cloneSets.ContainsKey(newSpace))
        {
            cloneSets.Add(newSpace, new_clone_list);

            new_clone_list.Next = new_clone_list;
            new_clone_list.Previous = new_clone_list;
        }
        else
        {
            CloneList current_new = cloneSets[newSpace];

            current_new.Next.Previous = new_clone_list;
            new_clone_list.Next = current_new.Next;
            current_new.Next = new_clone_list;
            new_clone_list.Previous = current_new;
        }

        if (clone == null)
        {
            currentSpace = newSpace;
        }
    }

    private void removeFromCurrentSet(CloneList clone)
    {
        BoxCollider2D cloneSpace = clone.Space;
        if (cloneSpace == null) return;

        if (clone.Next == clone) // only one
        {
            cloneSets.Remove(cloneSpace);
            return;
        }
        cloneSets.TryGetValue(cloneSpace, out CloneList c_clone_list);

        if (c_clone_list == clone) cloneSets[cloneSpace] = clone.Previous; // if this clone is the head of the clonelist its part of
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
            Next = current.Next,
            Space = currentSpace
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
