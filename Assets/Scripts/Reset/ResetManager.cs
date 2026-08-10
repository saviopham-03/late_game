using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Detects reset input and resets all objects that implement IResettable.
/// </summary>
public class ResetManager : MonoBehaviour
{
    private readonly List<IResettable> resettableObjects = new();

    private void Start()
    {
        FindResettableObjects();
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            ResetAll();
        }
    }

    private void FindResettableObjects()
    {
        resettableObjects.Clear();

        MonoBehaviour[] behaviours =
            FindObjectsByType<MonoBehaviour>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour is IResettable resettable)
            {
                resettableObjects.Add(resettable);
            }
        }

        Debug.Log(
            $"ResetManager found {resettableObjects.Count} resettable objects."
        );
    }

    public void ResetAll()
    {
        foreach (IResettable resettable in resettableObjects)
        {
            resettable.ResetState();
        }

        Debug.Log(
            $"Reset completed: {resettableObjects.Count} objects reset."
        );
    }
}