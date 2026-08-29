using UnityEngine;

public class PressurePlate : PuzzleInput
{
    [SerializeField] private Rigidbody2D plateBody;

    [SerializeField] private float activationDistance = 0.15f;

    private Vector2 restingPosition;

    private void Start()
    {
        restingPosition = plateBody.position;
    }

    private void FixedUpdate()
    {
        float pressedDistance =
            restingPosition.y - plateBody.position.y;

        if (pressedDistance >= activationDistance)
        {
            SetActive(true);
        }
        else
        {
            SetActive(false);
        }
    }

    public override void ResetPuzzleObject()
    {
        plateBody.position = restingPosition;
        plateBody.linearVelocity = Vector2.zero;
        plateBody.angularVelocity = 0f;

        SetActive(false);
    }
}