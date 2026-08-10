using UnityEngine;

public class PlayerColourController : MonoBehaviour
{
    [SerializeField] 
    private PlayerColour currentColour;

    public PlayerColour CurrentColour => currentColour;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public void SetColour(PlayerColour newColour)
    {
        currentColour = newColour;
    }
    
}
