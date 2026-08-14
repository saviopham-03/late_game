using UnityEngine;

public class PlayerColourController : MonoBehaviour
{
    Renderer renderer;
    void Start()
    {
        renderer = GetComponent<Renderer>();
    }
    [SerializeField] 
    private PlayerColour currentColour;

    public PlayerColour CurrentColour => currentColour;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public void SetColour(PlayerColour newColour)
    {
        Debug.Log("changed colour from " + currentColour + "to " + newColour);
        currentColour = newColour;
        renderer.material.color = PlayerColours.GetColor(currentColour);
        Debug.Log(renderer.material.color);
    }
    
}
