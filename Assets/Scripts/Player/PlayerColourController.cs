using UnityEngine;
using System;

public class PlayerColourController : MonoBehaviour
{
    private SpriteRenderer renderer;

    [SerializeField]
    private PlayerColour currentColour;

    public PlayerColour CurrentColour => currentColour;

    public event Action<PlayerColour> ColourChanged;

    private void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        SetColour(currentColour);
    }

    public void SetColour(PlayerColour newColour)
    {
        if (!renderer)
        {
            renderer = GetComponent<SpriteRenderer>();
        }

        Debug.Log("changed colour from " + currentColour + " to " + newColour);

        currentColour = newColour;
        renderer.material.color = PlayerColours.GetColor(currentColour);

        ColourChanged?.Invoke(currentColour);

        Debug.Log(renderer.material.color);
    }

    public PlayerColour GetColour()
    {
        return currentColour;
    }
}