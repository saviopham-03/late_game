using UnityEngine;
using UnityEngine.InputSystem;

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
    private CloneList current = new();

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
