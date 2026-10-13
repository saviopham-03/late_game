
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

    [Test]
    public void SetColour_UpdatesValueRendererAndRaisesEvent()
    {
        var go = new GameObject("test-player");
        Material testMaterial = null;

        try
        {
            // Create a SpriteRenderer
            var sprite = go.AddComponent<SpriteRenderer>();

            // Use a temporary material rather than modifying
            // an existing shared material asset.
            var shader = Shader.Find("Sprites/Default");
            Assert.IsNotNull(shader, "Sprites/Default shader not found.");

            testMaterial = new Material(shader);
            sprite.sharedMaterial = testMaterial;

            // Add the player colour controller
            var control = go.AddComponent<PlayerColourController>();

            int calls = 0;
            PlayerColour received = PlayerColour.Red;

            // Subscribe to the colour change event
            control.ColourChanged += c =>
            {
                calls++;
                received = c;
            };

            // Change the player colour to Blue
            control.SetColour(PlayerColour.Blue);

            // Verify the stored colour
            Assert.AreEqual(
                PlayerColour.Blue,
                control.GetColour()
            );

            Assert.AreEqual(
                PlayerColour.Blue,
                control.CurrentColour
            );

            // Verify the event received the correct colour
            Assert.AreEqual(
                PlayerColour.Blue,
                received
            );

            // Verify the event fired exactly once
            Assert.AreEqual(1, calls);

            // Verify the renderer changed colour
            Color expected = PlayerColour.Blue.GetColor();
            Color actual = sprite.color;

            // Compare RGBA channels with floating-point tolerance
            Assert.That(
                actual.r,
                Is.EqualTo(expected.r).Within(ColourTolerance)
            );

            Assert.That(
                actual.g,
                Is.EqualTo(expected.g).Within(ColourTolerance)
            );

            Assert.That(
                actual.b,
                Is.EqualTo(expected.b).Within(ColourTolerance)
            );

            Assert.That(
                actual.a,
                Is.EqualTo(expected.a).Within(ColourTolerance)
            );
        }
        finally
        {
            // Destroy the GameObject and temporary material
            Object.DestroyImmediate(go);

            if (testMaterial != null)
            {
                Object.DestroyImmediate(testMaterial);
            }
        }
    }
}
