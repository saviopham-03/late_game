using UnityEngine;

public class GrappleAnimatorScript : MonoBehaviour
{
    private Animator _animator;
    private SpriteRenderer _renderer;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.enabled = false;
    }

    public void InRange(bool in_range)
    {
        _animator.SetBool("in_range", in_range);
        _renderer.enabled = in_range;
    }

    public void Active(bool active)
    {
        _animator.SetBool("active", active);
    }
}