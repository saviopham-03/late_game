using System.Collections.Generic;
using UnityEngine;

public class Platform : PuzzleObject
{
    [Header("Core References")]
    [SerializeField] private Path path;
    [SerializeField] private PathModifier pathModifier;
    [SerializeField] private MovementBehaviour movementBehaviour;

    [Header("Modifiers")]
    [SerializeField] private PlatformModifier[] platformModifiers;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 2f;

    [Header("Puzzle Control")]
    [SerializeField] private bool puzzleMovementAllowed = true;

    private Rigidbody2D rb;

    private int currentNodeIndex;
    private int targetNodeIndex;

    private Vector2 startingPosition;
    private Quaternion startingRotation;

    private Vector2 platformVelocity = Vector2.zero;

    private readonly List<GameObject> riders = new List<GameObject>();

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("Platform requires a Rigidbody2D.");
            return;
        }

        if (path == null)
        {
            Debug.LogError("Platform requires a Path.");
            return;
        }

        if (pathModifier == null)
        {
            Debug.LogError("Platform requires a PathModifier.");
            return;
        }

        if (movementBehaviour == null)
        {
            Debug.LogError("Platform requires a MovementBehaviour.");
            return;
        }

        startingPosition = rb.position;
        startingRotation = transform.rotation;

        currentNodeIndex = path.GetStartNodeIndex();

        targetNodeIndex =
            pathModifier.GetNextNodeIndex(
                currentNodeIndex,
                path
            );

        movementBehaviour.Initialise(this);

        foreach (PlatformModifier modifier in platformModifiers)
        {
            if (modifier != null)
            {
                modifier.Initialise(this);
            }
        }
    }

    private void FixedUpdate()
    {
        foreach (PlatformModifier modifier in platformModifiers)
        {
            if (modifier != null)
            {
                modifier.UpdateModifier();
            }
        }

        if (!CanMove())
        {
            platformVelocity = Vector2.zero;
            return;
        }

        if (targetNodeIndex == -1)
        {
            platformVelocity = Vector2.zero;
            return;
        }

        Vector2 targetPosition =
            path.GetNodePosition(targetNodeIndex);

        Vector2 oldPosition = rb.position;

        Vector2 newPosition =
            Vector2.MoveTowards(
                oldPosition,
                targetPosition,
                movementSpeed * Time.fixedDeltaTime
            );

        Vector2 platformMovement =
            newPosition - oldPosition;

        platformVelocity =
            platformMovement / Time.fixedDeltaTime;

        rb.MovePosition(newPosition);

        if (Vector2.Distance(newPosition, targetPosition) < 0.01f)
        {
            NodeReached();
        }
    }

    private bool CanMove()
    {
        if (!movementBehaviour.ShouldMove())
        {
            return false;
        }

        if (!puzzleMovementAllowed)
        {
            return false;
        }

        foreach (PlatformModifier modifier in platformModifiers)
        {
            if (modifier != null && modifier.BlocksMovement())
            {
                return false;
            }
        }

        return true;
    }

    private void NodeReached()
    {
        currentNodeIndex = targetNodeIndex;

        movementBehaviour.OnNodeReached(currentNodeIndex);

        foreach (PlatformModifier modifier in platformModifiers)
        {
            if (modifier != null)
            {
                modifier.OnNodeReached(currentNodeIndex);
            }
        }

        targetNodeIndex =
            pathModifier.GetNextNodeIndex(
                currentNodeIndex,
                path
            );
    }

    public void RiderEnter(GameObject rider)
    {
        if (rider == null)
        {
            return;
        }

        if (riders.Contains(rider))
        {
            return;
        }

        riders.Add(rider);

        movementBehaviour.OnRiderEnter(rider);

        foreach (PlatformModifier modifier in platformModifiers)
        {
            if (modifier != null)
            {
                modifier.OnRiderEnter(rider);
            }
        }
    }

    public void RiderExit(GameObject rider)
    {
        if (rider == null)
        {
            return;
        }

        if (!riders.Contains(rider))
        {
            return;
        }

        riders.Remove(rider);

        movementBehaviour.OnRiderExit(rider);

        foreach (PlatformModifier modifier in platformModifiers)
        {
            if (modifier != null)
            {
                modifier.OnRiderExit(rider);
            }
        }
    }

    public int GetRiderCount()
    {
        return riders.Count;
    }

    public bool HasRiders()
    {
        return riders.Count > 0;
    }

    public Vector2 GetVelocity()
    {
        return platformVelocity;
    }

    public void SetPuzzleMovementAllowed(bool newState)
    {
        puzzleMovementAllowed = newState;

        if (!puzzleMovementAllowed)
        {
            platformVelocity = Vector2.zero;
        }

        foreach (PlatformModifier modifier in platformModifiers)
        {
            if (modifier != null)
            {
                modifier.OnPuzzleMovementChanged(newState);
            }
        }
    }

    public bool IsPuzzleMovementAllowed()
    {
        return puzzleMovementAllowed;
    }

    public override void ResetPuzzleObject()
    {
        rb.position = startingPosition;
        transform.rotation = startingRotation;

        platformVelocity = Vector2.zero;

        riders.Clear();

        path.ResetPuzzleObject();
        pathModifier.ResetPuzzleObject();
        movementBehaviour.ResetPuzzleObject();

        foreach (PlatformModifier modifier in platformModifiers)
        {
            if (modifier != null)
            {
                modifier.ResetPuzzleObject();
            }
        }

        currentNodeIndex =
            path.GetStartNodeIndex();

        targetNodeIndex =
            pathModifier.GetNextNodeIndex(
                currentNodeIndex,
                path
            );
    }
}