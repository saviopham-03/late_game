
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

        // Change the sprite tint without instantiating a material
        if (renderer != null)
        {
            renderer.color = PlayerColours.GetColor(currentColour);
            Debug.Log(renderer.color);
        }
        else
        {
            Debug.LogWarning("PlayerColourController: SpriteRenderer not found.");
        }

        // Notify other systems that the player's colour changed
        ColourChanged?.Invoke(currentColour);
    }

    public PlayerColour GetColour()
    {
        return currentColour;
    }
}
