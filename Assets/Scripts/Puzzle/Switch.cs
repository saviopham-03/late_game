using UnityEngine;

public class Switch : PuzzleInput
{
    public enum BehaviourType
    {
        TOGGLEABLE,
        SINGLE_USE
    }

    [SerializeField] private BehaviourType behaviourType = BehaviourType.TOGGLEABLE;

    [SerializeField] private float moveDistance = 0.1f;
    [SerializeField] private float moveSpeed = 2f;

    [SerializeField] private SpriteRenderer switchRenderer;

    private bool canTrigger = true;
    private bool hasBeenUsed = false;

    private Vector3 inactivePosition;
    private Vector3 activePosition;

    private void Start()
    {
        inactivePosition = transform.position;

        activePosition =
            inactivePosition + (Vector3.down * moveDistance);

        UpdateColour();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsValidActivator(other))
        {
            return;
        }

        if (!canTrigger)
        {
            return;
        }

        if (behaviourType == BehaviourType.TOGGLEABLE)
        {
            ToggleSwitch();
        }
        else if (behaviourType == BehaviourType.SINGLE_USE)
        {
            if (!hasBeenUsed)
            {
                SetSwitchState(true);
                hasBeenUsed = true;
            }
        }

        canTrigger = false;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsValidActivator(other))
        {
            canTrigger = true;
        }
    }

    private void ToggleSwitch()
    {
        SetSwitchState(!isActive);
    }

    private void SetSwitchState(bool newState)
    {
        SetActive(newState);
        UpdateColour();
    }

    private void UpdateColour()
    {
        if (switchRenderer == null)
        {
            return;
        }

        if (isActive)
        {
            switchRenderer.color = Color.green;
        }
        else
        {
            switchRenderer.color = Color.red;
        }
    }

    private bool IsValidActivator(Collider2D other)
    {
        return other.CompareTag("Player") ||
               other.CompareTag("Clone");
    }

    private void Update()
    {
        Vector3 targetPosition =
            isActive ? activePosition : inactivePosition;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );
    }

    public override void ResetPuzzleObject()
    {
        isActive = false;
        hasBeenUsed = false;
        canTrigger = true;

        transform.position = inactivePosition;

        UpdateColour();
        SendNotification();
    }
}