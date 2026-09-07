using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TilemapCollider2D))]
public class ColourTilemap : MonoBehaviour
{
    [SerializeField]
    private PlayerColour colour;

    private TilemapCollider2D tilemapCollider;

    public PlayerColour Colour => colour;
    public TilemapCollider2D Collider => tilemapCollider;

    private void Awake()
    {
        tilemapCollider = GetComponent<TilemapCollider2D>();
    }
}