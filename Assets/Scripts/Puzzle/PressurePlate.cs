using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : PuzzleInput
{
    [Header("Colour Interaction")]
    [Tooltip("When enabled, any coloured player/clone can activate the plate. HeavyObjects can also activate Universal plates.")]
    [SerializeField] private bool isUniversal = true;

    [Tooltip("The player colour required to activate this plate when it is not Universal.")]
    [SerializeField] private PlayerColour requiredColour = PlayerColour.Red;

    [Header("Pressure Plate")]
    [SerializeField] private Animator _animator;

    // Track individual colliders so multiple players/clones can stand on the
    // plate at the same time without one leaving incorrectly deactivating it.
    private readonly HashSet<Collider2D> objectsOnPlate = new();

    public bool IsUniversal => isUniversal;
    public PlayerColour RequiredColour => requiredColour;

    private void OnTriggerEnter2D(Collider2D other)
    {
        TrackActivator(other);
        EvaluatePlate();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Also recovers correctly if the plate/component becomes enabled while
        // an activator is already overlapping it.
        TrackActivator(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        objectsOnPlate.Remove(other);
        EvaluatePlate();
    }

    private void FixedUpdate()
    {
        // Re-evaluate continuously so changing a player's colour while they
        // remain on the pressure plate immediately affects activation.
        EvaluatePlate();
    }

    private void TrackActivator(Collider2D other)
    {
        if (IsPotentialActivator(other))
        {
            objectsOnPlate.Add(other);
        }
    }

    private bool IsPotentialActivator(Collider2D other)
    {
        if (other == null)
        {
            return false;
        }

        // Preserve the pressure plate's existing HeavyObject support.
        if (other.CompareTag("HeavyObject"))
        {
            return true;
        }

        // Players and clones are identified through their colour controller.
        // GetComponentInParent also supports colliders placed on child objects.
        return GetColourController(other) != null;
    }

    private void EvaluatePlate()
    {
        // Remove destroyed collider references.
        objectsOnPlate.RemoveWhere(collider => collider == null);

        bool shouldBeActive = false;

        foreach (Collider2D activator in objectsOnPlate)
        {
            if (IsCompatibleActivator(activator))
            {
                shouldBeActive = true;
                break;
            }
        }

        SetActive(shouldBeActive);

        if (_animator != null)
        {
            _animator.SetBool("active", shouldBeActive);
        }
    }

    private bool IsCompatibleActivator(Collider2D other)
    {
        if (other == null)
        {
            return false;
        }

        // Heavy objects have no PlayerColour, so they only activate plates
        // configured as Universal.
        if (other.CompareTag("HeavyObject"))
        {
            return isUniversal;
        }

        PlayerColourController colourController = GetColourController(other);

        if (colourController == null)
        {
            return false;
        }

        return isUniversal ||
               colourController.CurrentColour == requiredColour;
    }

    private PlayerColourController GetColourController(Collider2D other)
    {
        if (other == null)
        {
            return null;
        }

        PlayerColourController colourController =
            other.GetComponent<PlayerColourController>();

        if (colourController == null)
        {
            colourController =
                other.GetComponentInParent<PlayerColourController>();
        }

        return colourController;
    }

    public override void ResetPuzzleObject()
    {
        objectsOnPlate.Clear();
        SetActive(false);

        if (_animator != null)
        {
            _animator.SetBool("active", false);
        }
    }
}
