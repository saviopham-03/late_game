using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigationManager : MonoBehaviour
{   
    [SerializeField] private float transitionTime = 1f;
    public static SceneNavigationManager Instance { get; private set; }

    [SerializeField]
    private List<string> orderedScenes = new List<string>();
    private TransitionLevel transition;

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

    private void Start()
    {
        transition = transform.GetChild(0).gameObject.GetComponent<TransitionLevel>();
    }

    /// <summary>
    /// Loads a specific scene using its scene name.
    /// </summary>
    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning("Scene name cannot be empty.");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogWarning(
                $"Scene '{sceneName}' cannot be loaded. " +
                "Make sure it is included in the Build Settings."
            );
            return;
        }

        StartCoroutine(LoadAnimation(sceneName));
    }

    IEnumerator LoadAnimation(string sceneName)
    {
        transition.start = true;
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneName);
        transition.start = true;
    }

    /// <summary>
    /// Loads the next scene in orderedScenes.
    /// </summary>
    public void LoadNextScene()
    {
        LoadRelativeScene(1);
    }

    /// <summary>
    /// Loads the previous scene in orderedScenes.
    /// </summary>
    public void LoadPreviousScene()
    {
        LoadRelativeScene(-1);
    }

    /// <summary>
    /// Reloads the currently active scene.
    /// </summary>
    public void ReloadCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        LoadScene(currentScene);
    }

    private void LoadRelativeScene(int direction)
    {
        if (orderedScenes == null || orderedScenes.Count == 0)
        {
            Debug.LogWarning("The ordered scene list is empty.");
            return;
        }

        string currentScene = SceneManager.GetActiveScene().name;
        int currentIndex = orderedScenes.IndexOf(currentScene);

        if (currentIndex == -1)
        {
            Debug.LogWarning(
                $"Current scene '{currentScene}' is not in the ordered scene list."
            );
            return;
        }

        int targetIndex = currentIndex + direction;

        if (targetIndex < 0 || targetIndex >= orderedScenes.Count)
        {
            Debug.LogWarning("There is no scene in that direction.");
            return;
        }

        LoadScene(orderedScenes[targetIndex]);
    }
}