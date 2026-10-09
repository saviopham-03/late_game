using UnityEngine;

public class GrapplePoint : MonoBehaviour
{
    public enum GrappleType
    {
        Swing,
        Pull
    }

    [SerializeField]
    private GrappleType grappleType = GrappleType.Swing;

    public GrappleType Type => grappleType;
}