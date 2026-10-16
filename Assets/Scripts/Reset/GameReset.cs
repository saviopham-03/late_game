using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public static class GameReset
{
    public static void ResetGame()
    {
        PersistentObject.DestroyAll();

        ResetCoroutineRunner.Run();
    }
}

public class ResetCoroutineRunner : MonoBehaviour
{
    private static ResetCoroutineRunner instance;

    public static void Run()
    {
        if (instance == null)
        {
            GameObject obj = new GameObject("ResetCoroutineRunner");
            instance = obj.AddComponent<ResetCoroutineRunner>();
            DontDestroyOnLoad(obj);
        }

        instance.StartCoroutine(ResetCoroutine());
    }

    private static IEnumerator ResetCoroutine()
    {
        // Wait for persistent objects to be destroyed.
        yield return null;

        SceneManager.LoadScene("MainMenu");
    }
}