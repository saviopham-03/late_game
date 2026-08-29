using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SceneNavigationManager))]
public class SceneNavigationTestControls : MonoBehaviour
{
    private SceneNavigationManager navigationManager;

    private void Awake()
    {
        navigationManager = GetComponent<SceneNavigationManager>();
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(
            new Rect(20, 20, 260, 310),
            GUI.skin.box
        );

        GUILayout.Label(
            $"Current Scene: {SceneManager.GetActiveScene().name}"
        );

        if (GUILayout.Button("Load Test A", GUILayout.Height(40)))
        {
            navigationManager.LoadScene("SceneNavigationTestA");
        }

        if (GUILayout.Button("Load Test B", GUILayout.Height(40)))
        {
            navigationManager.LoadScene("SceneNavigationTestB");
        }

        if (GUILayout.Button("Previous Scene", GUILayout.Height(40)))
        {
            navigationManager.LoadPreviousScene();
        }

        if (GUILayout.Button("Next Scene", GUILayout.Height(40)))
        {
            navigationManager.LoadNextScene();
        }

        if (GUILayout.Button("Reload Current Scene", GUILayout.Height(40)))
        {
            navigationManager.ReloadCurrentScene();
        }

        GUILayout.EndArea();
    }
}