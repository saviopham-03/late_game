using UnityEngine;

public class GrappleAnimatorScript : MonoBehaviour
{
    [SerializeField] float dimValue;
    private Color grappleBaseColour = new Color(1f, 1f, 1f);
    private Animator _animator;
    private SpriteRenderer _renderer;
    private Color outOfRangeColour;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        outOfRangeColour = new Color(dimValue, dimValue, dimValue);
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.material.color = outOfRangeColour;
    }

    public void InRange(bool in_range)
    {
        _animator.SetBool("in_range", in_range);
        _renderer.material.color = in_range ? grappleBaseColour : outOfRangeColour;
    }
    public void Active(bool active)
    {
        _animator.SetBool("active", active);
    }
}
