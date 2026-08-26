using UnityEngine;

public class DisappearingModifier : PlatformModifier
{
    public enum TriggerMode
    {
        RiderEnter,
        PuzzleActivated,
        Either
    }

    [Header("Trigger")]
    [SerializeField] private TriggerMode triggerMode = TriggerMode.RiderEnter;

    [Header("Timing")]
    [SerializeField] private float disappearDelay = 1f;
    [SerializeField] private float respawnDelay = 2f;

    [Header("Behaviour")]
    [SerializeField] private bool pauseMovementWhileHidden = true;

    [Header("References")]
    [SerializeField] private Renderer platformRenderer;
    [SerializeField] private Collider2D platformCollider;

    private float disappearTimer;
    private float respawnTimer;

    private bool disappearanceTriggered;
    private bool isHidden;

    public override void Initialise(Platform platformReference)
    {
        base.Initialise(platformReference);

        if (platformRenderer == null)
        {
            platformRenderer = GetComponent<Renderer>();
        }

        if (platformCollider == null)
        {
            platformCollider = GetComponent<Collider2D>();
        }
    }

    public override void OnRiderEnter(GameObject rider)
    {
        if (
            triggerMode == TriggerMode.RiderEnter ||
            triggerMode == TriggerMode.Either
        )
        {
            TriggerDisappearance();
        }
    }

    public override void OnPuzzleMovementChanged(bool movementAllowed)
    {
        if (
            triggerMode != TriggerMode.PuzzleActivated &&
            triggerMode != TriggerMode.Either
        )
        {
            return;
        }

        if (movementAllowed)
        {
            TriggerDisappearance();
        }
    }

    private void TriggerDisappearance()
    {
        if (disappearanceTriggered)
        {
            return;
        }

        disappearanceTriggered = true;
        disappearTimer = disappearDelay;
    }

    public override void UpdateModifier()
    {
        if (!disappearanceTriggered)
        {
            return;
        }

        if (!isHidden)
        {
            disappearTimer -= Time.fixedDeltaTime;

            if (disappearTimer <= 0f)
            {
                HidePlatform();
            }

            return;
        }

        respawnTimer -= Time.fixedDeltaTime;

        if (respawnTimer <= 0f)
        {
            ShowPlatform();
        }
    }

    private void HidePlatform()
    {
        isHidden = true;

        if (platformRenderer != null)
        {
            platformRenderer.enabled = false;
        }

        if (platformCollider != null)
        {
            platformCollider.enabled = false;
        }

        respawnTimer = respawnDelay;
    }

    private void ShowPlatform()
    {
        isHidden = false;
        disappearanceTriggered = false;

        if (platformRenderer != null)
        {
            platformRenderer.enabled = true;
        }

        if (platformCollider != null)
        {
            platformCollider.enabled = true;
        }

        disappearTimer = 0f;
        respawnTimer = 0f;
    }

    public override bool BlocksMovement()
    {
        return pauseMovementWhileHidden && isHidden;
    }

    public override void ResetPuzzleObject()
    {
        disappearTimer = 0f;
        respawnTimer = 0f;

        disappearanceTriggered = false;
        isHidden = false;

        if (platformRenderer != null)
        {
            platformRenderer.enabled = true;
        }

        if (platformCollider != null)
        {
            platformCollider.enabled = true;
        }
    }
}