using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]

public class Hitbox : MonoBehaviour
{
    [SerializeField]
    private bool isTrigger = true;
    private BoxCollider2D boxCollider;
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.isTrigger = isTrigger;
    }
}
