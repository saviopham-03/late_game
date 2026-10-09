using UnityEngine;

public class TransitionLevel : MonoBehaviour
{
    public bool start = false;
    public Animator _animator;
    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            _animator.SetTrigger("start");
            start = false;
        }
    }
}
