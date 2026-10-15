using UnityEngine;
using System.Collections.Generic;

public class PersistentObject : MonoBehaviour
{
    private static readonly HashSet<PersistentObject> instances = new();

    private void Awake()
    {
        instances.Add(this);
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        instances.Remove(this);
    }

    public static void DestroyAll()
    {
        foreach (PersistentObject obj in instances)
        {
            if (obj != null) Destroy(obj.gameObject);
        }

        instances.Clear();
    }
}