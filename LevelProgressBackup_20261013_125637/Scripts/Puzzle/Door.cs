using UnityEngine;

public class Door : PuzzleOutput
{
    [SerializeField] private float openHeight = 3f;
    [SerializeField] private float moveSpeed = 2f;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private Vector3 targetPosition;

    private void Start()
    {
        closedPosition = transform.position;

        openPosition =
            closedPosition + (Vector3.up * openHeight);

        if (startingState)
        {
            currentState = true;
            targetPosition = openPosition;

            // Start immediately in the open position
            transform.position = openPosition;
        }
        else
        {
            currentState = false;
            targetPosition = closedPosition;

            transform.position = closedPosition;
        }
    }

    protected override void Activate()
    {
        targetPosition = openPosition;
    }

    protected override void Deactivate()
    {
        targetPosition = closedPosition;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );
    }
}