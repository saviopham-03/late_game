using UnityEngine;

public class SceneTransitionObject : MonoBehaviour
{
    [SerializeField] string newScene;
    private void OnTriggerEnter2D(Collider2D obj)
    {
        SceneNavigationManager.Instance.LoadScene(newScene);
    }
}
