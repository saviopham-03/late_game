using UnityEngine;

public class BGM : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static BGM Instance { get; private set; }
    private void Awake()
    {
        // Keep only one SceneNavigationManager between scenes.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
