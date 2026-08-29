using UnityEngine;

public class PlayerColourController : MonoBehaviour
{
    SpriteRenderer renderer;
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        SetColour(currentColour);
    }
    [SerializeField] 
    private PlayerColour currentColour;

    public PlayerColour CurrentColour => currentColour;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public void SetColour(PlayerColour newColour)
    {
        if (!renderer)
        {
            renderer = GetComponent<SpriteRenderer>();
        }
        Debug.Log("changed colour from " + currentColour + "to " + newColour);
        currentColour = newColour;
        renderer.material.color = PlayerColours.GetColor(currentColour);
        Debug.Log(renderer.material.color);
    }

    public PlayerColour GetColour()
    {
        return currentColour;
    }
    
}
