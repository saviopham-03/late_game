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
    [SerializeField] private bool returnToStartOnDeactivation = true;

    [Header("Puzzle Control")]
    [SerializeField] private bool puzzleMovementAllowed = true;
    

    private Rigidbody2D rb;

    private int currentNodeIndex;
    private int nextNodeIndex;
    private int previousNodeIndex;

    private Vector2 startingPosition;
    private Quaternion startingRotation;

    private Vector2 platformVelocity = Vector2.zero;

    private readonly List<GameObject> riders = new List<GameObject>();

    private bool goForward = true;
    private enum PathDirection{behind, on_node, ahead};
    private PathDirection direction;

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
        previousNodeIndex = currentNodeIndex;
        direction = PathDirection.on_node;

        nextNodeIndex =
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
        // Debug.Log((previousNodeIndex, currentNodeIndex, nextNodeIndex, direction));
        goForward = !returnToStartOnDeactivation || puzzleMovementAllowed;
        
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
            if (!returnToStartOnDeactivation) return;
        }

        if (nextNodeIndex == -1 && direction == PathDirection.on_node)
        {
            platformVelocity = Vector2.zero;
            if (goForward) return;
        }
        int targetIndex = GetTargetPosition();
        Vector2 targetPosition = path.GetNodePosition(targetIndex);
        
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

        if (HasRiders())
        {
            foreach (GameObject rider in riders)
            {
                rider.GetComponent<Rigidbody2D>().position += platformMovement;
            }
        }

        if (Vector2.Distance(newPosition, targetPosition) < 0.01f)
        {
            NodeReached(targetIndex);
        }
    }

    private int GetTargetPosition()
    {
        if (goForward)
        {
            if (direction == PathDirection.ahead || direction == PathDirection.on_node) {
                direction = PathDirection.ahead;
                return nextNodeIndex;
            }
            return currentNodeIndex;
        } 
        // going backwards
        if (direction == PathDirection.behind || direction == PathDirection.on_node) {
            direction = PathDirection.behind;
            return previousNodeIndex;
        }
        return currentNodeIndex;
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

    private void NodeReached(int nodeReached)
    {
        direction = PathDirection.on_node;
        currentNodeIndex = nodeReached;

        movementBehaviour.OnNodeReached(currentNodeIndex);

        foreach (PlatformModifier modifier in platformModifiers)
        {
            if (modifier != null)
            {
                modifier.OnNodeReached(currentNodeIndex);
            }
        }

        nextNodeIndex =
            pathModifier.GetNextNodeIndex(
                currentNodeIndex,
                path
            );
        previousNodeIndex =
            path.GetPreviousNodeIndex(
                currentNodeIndex);
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

        nextNodeIndex =
            pathModifier.GetNextNodeIndex(
                currentNodeIndex,
                path
            );
    }

    void OnCollisionEnter2D(Collision2D obj)
    {
        if (obj.gameObject.CompareTag("Player") || obj.gameObject.CompareTag("Heavy Object"))
        {
            RiderEnter(obj.gameObject);
        }
    }

    void OnCollisionExit2D(Collision2D obj)
    {
        if (obj.gameObject.CompareTag("Player") || obj.gameObject.CompareTag("Heavy Object")) RiderExit(obj.gameObject);
    }
}