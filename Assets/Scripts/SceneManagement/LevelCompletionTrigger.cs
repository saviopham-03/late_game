using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Add to an exit object with an Is Trigger collider; player must have the Player tag.</summary>
[RequireComponent(typeof(Collider2D))]
public class LevelCompletionTrigger : MonoBehaviour
{
    [Range(1, LevelProgressManager.TotalLevels)]
    [SerializeField] private int levelNumber = 1;
    [SerializeField] private string levelSelectScene = "LevelSelect";
    private bool completed;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (completed || !other.CompareTag("Player")) return;
        if (!LevelProgressManager.IsUnlocked(levelNumber)) return;
        if (!Application.CanStreamedLevelBeLoaded(levelSelectScene))
        {
            Debug.LogError($"Add '{levelSelectScene}' to the enabled Build Profiles scene list.", this);
            return;
        }
        completed = true;
        LevelProgressManager.CompleteLevel(levelNumber);
        if (SceneNavigationManager.Instance != null)
            SceneNavigationManager.Instance.LoadScene(levelSelectScene);
        else
            SceneManager.LoadScene(levelSelectScene);
    }

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }
}
