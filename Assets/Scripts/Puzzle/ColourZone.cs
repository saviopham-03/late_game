using System.Collections.Generic;
using UnityEngine;

public class ColourZone : PuzzleInput
{
    [Header("Colour Requirement")]
    [SerializeField] private bool isUniversal = false;
    [SerializeField] private PlayerColour requiredColour = PlayerColour.Red;

    [Header("Visual State")]
    [SerializeField] private SpriteRenderer zoneRenderer;

    [Tooltip(
        "When enabled, an inactive non-universal zone automatically uses " +
        "the colour selected in Required Colour."
    )]
    [SerializeField] private bool useRequiredColourWhenInactive = true;

    [Tooltip(
        "Used while the zone is inactive when " +
        "Use Required Colour When Inactive is disabled."
    )]
    [SerializeField] private Color inactiveColour = Color.red;

    [Tooltip("Inactive colour used for a Universal zone.")]
    [SerializeField] private Color universalInactiveColour = Color.white;

    [Tooltip("Colour displayed while the zone is active.")]
    [SerializeField] private Color activeColour = Color.green;

    // Track colliders rather than only GameObjects so the zone also behaves
    // correctly if a character later has more than one collider.
    private readonly HashSet<Collider2D> overlappingCharacterColliders = new();

    public bool IsUniversal => isUniversal;
    public PlayerColour RequiredColour => requiredColour;

    private void Awake()
    {
        TryAssignRenderer();
        UpdateVisual();
    }

    private void Start()
    {
        EvaluateZone();
    }

    private void FixedUpdate()
    {
        // Re-evaluate continuously so changing a player's colour while they
        // remain inside the zone immediately affects the puzzle input.
        EvaluateZone();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TrackCharacterCollider(other);
        EvaluateZone();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // This also recovers correctly if the zone/component becomes enabled
        // while a character is already overlapping it.
        TrackCharacterCollider(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        overlappingCharacterColliders.Remove(other);
        EvaluateZone();
    }

    private void TrackCharacterCollider(Collider2D other)
    {
        if (GetColourController(other) != null)
        {
            overlappingCharacterColliders.Add(other);
        }
    }

    private void EvaluateZone()
    {
        // Remove references to colliders whose GameObjects have been destroyed.
        overlappingCharacterColliders.RemoveWhere(
            collider => collider == null
        );

        bool shouldBeActive = false;

        foreach (Collider2D characterCollider in overlappingCharacterColliders)
        {
            PlayerColourController colourController =
                GetColourController(characterCollider);

            if (colourController != null &&
                IsColourCompatible(colourController))
            {
                shouldBeActive = true;
                break;
            }
        }

        SetZoneState(shouldBeActive);
    }

    private bool IsColourCompatible(
        PlayerColourController colourController
    )
    {
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

    private void SetZoneState(bool newState)
    {
        if (isActive == newState)
        {
            return;
        }

        // PuzzleInput.SetActive notifies the assigned PuzzleController.
        SetActive(newState);
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (zoneRenderer == null)
        {
            return;
        }

        zoneRenderer.color =
            isActive ? activeColour : GetInactiveVisualColour();
    }

    private Color GetInactiveVisualColour()
    {
        if (isUniversal)
        {
            return universalInactiveColour;
        }

        if (useRequiredColourWhenInactive)
        {
            return requiredColour.GetColor();
        }

        return inactiveColour;
    }

    private void TryAssignRenderer()
    {
        if (zoneRenderer == null)
        {
            zoneRenderer = GetComponent<SpriteRenderer>();
        }
    }

    private void OnValidate()
    {
        // Makes Inspector colour changes visible in Edit Mode without
        // requiring the game to be running.
        TryAssignRenderer();
        UpdateVisual();
    }

    private void Reset()
    {
        TryAssignRenderer();
        UpdateVisual();
    }

    public override void ResetPuzzleObject()
    {
        overlappingCharacterColliders.Clear();

        if (isActive)
        {
            SetActive(false);
        }

        UpdateVisual();
    }
}
