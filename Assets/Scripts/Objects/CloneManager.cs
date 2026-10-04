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
    [SerializeField] private InputActionReference switchAction;

    [SerializeField] GameObject player;
    private CloneList current = new();

    void Start()
    {
        current.Value = player;
        current.Next = current;
        current.Previous = current;
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

    private void OnEnable()
    {
        switchAction.action.started += OnSwitchStarted;
    }

    private void OnDisable()
    {
        switchAction.action.started -= OnSwitchStarted;
    }

    private void OnSwitchStarted(InputAction.CallbackContext context)
    {
        if (Time.timeScale == 0f || current.Value == null) return;
        switchClone();
    }
}
