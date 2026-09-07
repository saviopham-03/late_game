using UnityEngine;

[RequireComponent(typeof(PlayerColourController))]
public class PlayerColourCollision : MonoBehaviour
{
    private PlayerColourController colourController;
    private Collider2D playerCollider;
    private ColourTilemap[] colourTilemaps;

    private void Start()
    {
        colourController = GetComponent<PlayerColourController>();
        playerCollider = GetComponent<Collider2D>();

        colourTilemaps = FindObjectsByType<ColourTilemap>(
            FindObjectsSortMode.None
        );

        colourController.ColourChanged += OnColourChanged;

        UpdateCollisions(colourController.CurrentColour);
    }

    private void OnDestroy()
    {
        if (colourController != null)
        {
            colourController.ColourChanged -= OnColourChanged;
        }
    }

    private void OnColourChanged(PlayerColour newColour)
    {
        UpdateCollisions(newColour);
    }

    private void UpdateCollisions(PlayerColour playerColour)
    {
        foreach (ColourTilemap tilemap in colourTilemaps)
        {
            bool sameColour = tilemap.Colour == playerColour;

            Physics2D.IgnoreCollision(
                playerCollider,
                tilemap.Collider,
                sameColour
            );
        }
    }
}