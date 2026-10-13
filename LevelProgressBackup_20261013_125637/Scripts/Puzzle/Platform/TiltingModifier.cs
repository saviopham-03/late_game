using System.Collections.Generic;
using UnityEngine;

public class TiltingModifier : PlatformModifier
{
    [Header("Tilt Settings")]
    [SerializeField] private float maxTiltAngle = 10f;
    [SerializeField] private float tiltSpeed = 5f;
    [SerializeField] private float returnSpeed = 5f;

    private readonly List<GameObject> riders = new List<GameObject>();

    private float neutralAngle;

    public override void Initialise(Platform platformReference)
    {
        base.Initialise(platformReference);

        neutralAngle = platform.transform.eulerAngles.z;
    }

    public override void OnRiderEnter(GameObject rider)
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
    }

    public override void OnRiderExit(GameObject rider)
    {
        if (rider == null)
        {
            return;
        }

        riders.Remove(rider);
    }

    public override void UpdateModifier()
    {
        if (riders.Count == 0)
        {
            RotateTowards(
                neutralAngle,
                returnSpeed
            );

            return;
        }

        float averageRelativeX = 0f;
        int validRiders = 0;

        foreach (GameObject rider in riders)
        {
            if (rider == null)
            {
                continue;
            }

            float relativeX =
                rider.transform.position.x
                - platform.transform.position.x;

            averageRelativeX += relativeX;
            validRiders++;
        }

        if (validRiders == 0)
        {
            RotateTowards(
                neutralAngle,
                returnSpeed
            );

            return;
        }

        averageRelativeX /= validRiders;

        float desiredAngle = neutralAngle;

        if (averageRelativeX < 0f)
        {
            // Rider is on the left,
            // so the left side should tilt downward.
            desiredAngle =
                neutralAngle + maxTiltAngle;
        }
        else if (averageRelativeX > 0f)
        {
            // Rider is on the right,
            // so the right side should tilt downward.
            desiredAngle =
                neutralAngle - maxTiltAngle;
        }

        RotateTowards(
            desiredAngle,
            tiltSpeed
        );
    }

    private void RotateTowards(
        float targetAngle,
        float speed
    )
    {
        float currentAngle =
            platform.transform.eulerAngles.z;

        float newAngle =
            Mathf.MoveTowardsAngle(
                currentAngle,
                targetAngle,
                speed * Time.fixedDeltaTime
            );

        platform.transform.rotation =
            Quaternion.Euler(
                0f,
                0f,
                newAngle
            );
    }

    public override void ResetPuzzleObject()
    {
        riders.Clear();

        platform.transform.rotation =
            Quaternion.Euler(
                0f,
                0f,
                neutralAngle
            );
    }
}