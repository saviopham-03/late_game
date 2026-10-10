using UnityEngine;

public class Switch : PuzzleInput
{
    public enum BehaviourType
    {
        TOGGLEABLE,
        SINGLE_USE
    }

    [SerializeField] private Animator _animator;
    [SerializeField] private BehaviourType behaviourType = BehaviourType.TOGGLEABLE;

    [Header("Colour Interaction")]
    [SerializeField] private bool isUniversal = true;
    [SerializeField] private PlayerColour switchColour = PlayerColour.Red;

    [SerializeField] private float moveDistance = 0.1f;
    [SerializeField] private float moveSpeed = 2f;

    [SerializeField] private SpriteRenderer switchRenderer;

    private bool canTrigger = true;
    private bool hasBeenUsed = false;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        UpdateColour();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsValidActivator(other))
        {
            return;
        }

        if (!IsColourCompatible(other))
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
        if (_animator) _animator.SetBool("active", isActive);
    }

    private bool IsValidActivator(Collider2D other)
    {
        return other.CompareTag("Player") ||
               other.CompareTag("Clone");
    }

    private bool IsColourCompatible(Collider2D other)
    {
        if (isUniversal)
        {
            return true;
        }

        PlayerColourController colourController =
            other.GetComponent<PlayerColourController>();

        if (colourController == null)
        {
            colourController = other.GetComponentInParent<PlayerColourController>();
        }

        return colourController != null &&
               colourController.CurrentColour == switchColour;
    }

    public override void ResetPuzzleObject()
    {
        isActive = false;
        hasBeenUsed = false;
        canTrigger = true;

        UpdateColour();
        SendNotification();
    }
}