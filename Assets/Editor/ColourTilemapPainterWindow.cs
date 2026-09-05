using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ColourTilemapPainterWindow : EditorWindow
{
    [MenuItem("Tools/Colour Tilemap Painter")]
    public static void ShowWindow()
    {
        GetWindow<ColourTilemapPainterWindow>("Colour Tilemap Painter");
    }

    private void OnGUI()
    {
        GUILayout.Label("Paint Colour", EditorStyles.boldLabel);

        if (GUILayout.Button("Red"))
            SelectColour(PlayerColour.Red);

        if (GUILayout.Button("Blue"))
            SelectColour(PlayerColour.Blue);

        if (GUILayout.Button("Green"))
            SelectColour(PlayerColour.Green);

        if (GUILayout.Button("Yellow"))
            SelectColour(PlayerColour.Yellow);

        GUILayout.Space(15);

        GUILayout.Label("Recolour Selection", EditorStyles.boldLabel);

        if (GUILayout.Button("Recolour Red"))
            RecolourSelection(PlayerColour.Red);

        if (GUILayout.Button("Recolour Blue"))
            RecolourSelection(PlayerColour.Blue);

        if (GUILayout.Button("Recolour Green"))
            RecolourSelection(PlayerColour.Green);

        if (GUILayout.Button("Recolour Yellow"))
            RecolourSelection(PlayerColour.Yellow);
    }

    private void SelectColour(PlayerColour colour)
    {
        ColourTilemap tilemap = FindColourTilemap(colour);

        if (tilemap == null)
        {
            Debug.LogWarning($"No ColourTilemap found for {colour}.");
            return;
        }

        Tile tile = LoadTile(colour);

        if (tile == null)
            return;

        Selection.activeGameObject = tilemap.gameObject;
        GridPaintingState.scenePaintTarget = tilemap.gameObject;

        if (GridPaintingState.gridBrush is GridBrush brush)
        {
            brush.Init(Vector3Int.one);
            brush.SetTile(Vector3Int.zero, tile);
        }
    }

    private void RecolourSelection(PlayerColour newColour)
    {
        if (!GridSelection.active)
        {
            Debug.LogWarning("No Tilemap cells are selected.");
            return;
        }

        GameObject sourceObject = GridSelection.target;

        if (sourceObject == null)
        {
            Debug.LogWarning("Could not find the selected Tilemap.");
            return;
        }

        Tilemap sourceTilemap = sourceObject.GetComponent<Tilemap>();

        if (sourceTilemap == null)
        {
            Debug.LogWarning("The current selection is not on a Tilemap.");
            return;
        }

        ColourTilemap destinationColourTilemap = FindColourTilemap(newColour);

        if (destinationColourTilemap == null)
        {
            Debug.LogWarning($"No ColourTilemap found for {newColour}.");
            return;
        }

        Tilemap destinationTilemap =
            destinationColourTilemap.GetComponent<Tilemap>();

        Tile newTile = LoadTile(newColour);

        if (newTile == null)
            return;

        BoundsInt selection = GridSelection.position;

        Undo.RecordObject(sourceTilemap, "Recolour Tiles");
        Undo.RecordObject(destinationTilemap, "Recolour Tiles");

        foreach (Vector3Int position in selection.allPositionsWithin)
        {
            if (!sourceTilemap.HasTile(position))
                continue;

            sourceTilemap.SetTile(position, null);
            destinationTilemap.SetTile(position, newTile);
        }

        EditorUtility.SetDirty(sourceTilemap);
        EditorUtility.SetDirty(destinationTilemap);

        GridSelection.Clear();
    }

    private ColourTilemap FindColourTilemap(PlayerColour colour)
    {
        ColourTilemap[] tilemaps = FindObjectsByType<ColourTilemap>(
            FindObjectsSortMode.None
        );

        foreach (ColourTilemap tilemap in tilemaps)
        {
            if (tilemap.Colour == colour)
                return tilemap;
        }

        return null;
    }

    private Tile LoadTile(PlayerColour colour)
    {
        string path = $"Assets/Tiles/{colour}Tile.asset";

        Tile tile = AssetDatabase.LoadAssetAtPath<Tile>(path);

        if (tile == null)
        {
            Debug.LogWarning($"No tile asset found at {path}.");
        }

        return tile;
    }
}//