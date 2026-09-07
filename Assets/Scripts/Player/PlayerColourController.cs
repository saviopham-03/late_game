using UnityEngine;
using System;

public class PlayerColourController : MonoBehaviour
{
    private Renderer renderer;

    [SerializeField]
    private PlayerColour currentColour;

    public PlayerColour CurrentColour => currentColour;

    public event Action<PlayerColour> ColourChanged;

    private void Start()
    {
        renderer = GetComponent<Renderer>();
        SetColour(currentColour);
    }

    public void SetColour(PlayerColour newColour)
    {
        if (!renderer)
        {
            renderer = GetComponent<Renderer>();
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