using UnityEngine;
public enum PlayerColour
{
    Red,
    Blue,
    Green,
    Yellow
}

public static class PlayerColours
{
    public static Color GetColor(this PlayerColour color) => color switch
    {
        PlayerColour.Red => new Color(255/255f, 18/255f, 0/255f),
        PlayerColour.Blue => new Color(66/255f, 148/255f, 241/255f),
        PlayerColour.Green => new Color(48/255f, 185/255f, 30/255f),
        PlayerColour.Yellow => new Color(230/255f, 214/255f, 49/255f),
        _ => throw new System.Exception(nameof(color))
    };
}

