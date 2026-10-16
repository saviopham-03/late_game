
using NUnit.Framework;
using UnityEngine;

public class ColourTests
{
    private const float ColourTolerance = 0.001f;

    [TestCase(PlayerColour.Red, 1f, 18f / 255f, 0f)]
    [TestCase(PlayerColour.Blue, 66f / 255f, 148f / 255f, 241f / 255f)]
    [TestCase(PlayerColour.Green, 48f / 255f, 185f / 255f, 30f / 255f)]
    [TestCase(PlayerColour.Yellow, 230f / 255f, 214f / 255f, 49f / 255f)]
    public void ColourMapping_ReturnsConfiguredRGBA(
        PlayerColour colour,
        float r,
        float g,
        float b)
    {
        Color actual = colour.GetColor();

        Assert.That(actual.r, Is.EqualTo(r).Within(ColourTolerance));
        Assert.That(actual.g, Is.EqualTo(g).Within(ColourTolerance));
        Assert.That(actual.b, Is.EqualTo(b).Within(ColourTolerance));
        Assert.That(actual.a, Is.EqualTo(1f).Within(ColourTolerance));
    }

    [Test]
    public void ColourMapping_InvalidEnumThrows()
    {
        Assert.Throws<System.Exception>(() =>
            ((PlayerColour)999).GetColor());
    }
}
