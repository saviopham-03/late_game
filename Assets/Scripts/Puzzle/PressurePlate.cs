using UnityEngine;

public class PressurePlate : PuzzleInput
{
    // [SerializeField] private Rigidbody2D plateBody;

    // [SerializeField] private float activationDistance = 0.15f;
    [SerializeField] private Animator _animator;

    private Vector2 restingPosition;
    private int objects_on_plate;

    private void Start()
    {
        // restingPosition = plateBody.position;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        objects_on_plate++;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        objects_on_plate--;
    }

    private void FixedUpdate()
    {
        // float pressedDistance =
        //     restingPosition.y - plateBody.position.y;
        SetActive(objects_on_plate != 0);
        _animator.SetBool("active", objects_on_plate != 0);
        // if (pressedDistance >= activationDistance)
        
    //     if (objects_on_plate != 0)
    //     {
    //         SetActive(true);
    //     }
    //     else
    //     {
    //         SetActive(false);
    //     }
    }

    public override void ResetPuzzleObject()
    {
        // plateBody.position = restingPosition;
        // plateBody.linearVelocity = Vector2.zero;
        // plateBody.angularVelocity = 0f;

        SetActive(false);
    }
}