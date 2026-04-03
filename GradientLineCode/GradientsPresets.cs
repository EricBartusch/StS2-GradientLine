using Godot;

namespace GradientLine.GradientLineCode;

public class GradientsPresets
{
    public static readonly Color[] FireColors =
    [
        new Color(0.1f, 0f, 0f),
        new Color(0.8f, 0.1f, 0f),
        new Color(1f, 0.5f, 0f),
        new Color(1f, 0.9f, 0.2f),
        new Color(1f, 1f, 0.8f),
        new Color(0.1f, 0f, 0f)
    ];

    public static readonly Color[] OceanColors =
    [
        new Color(0f, 0.05f, 0.2f),
        new Color(0f, 0.3f, 0.6f),
        new Color(0f, 0.7f, 0.9f),
        new Color(0.4f, 0.9f, 1f),
        new Color(0.8f, 1f, 1f),
        new Color(0f, 0.05f, 0.2f)
    ];

    public static readonly Color[] MonoColors =
    [
        new Color(0.1f, 0.1f, 0.1f),
        new Color(1f, 1f, 1f),
        new Color(0.1f, 0.1f, 0.1f)
    ];
    
    public static readonly Color[] ChristmasColors =
    [
        new Color(1f, 0f, 0f),
        new Color(1f, 1f, 1f),
        new Color(0f, 0.5f, 0f),
        new Color(0f, 0.5f, 0f),
        new Color(1f, 1f, 1f),
        new Color(1f, 0f, 0f)
    ];
    
    public static readonly Color[] SpringColors =
    {
        new Color(1f, 0.8f, 0.9f), // pastel pink
        new Color(0.9f, 0.9f, 1f), // pastel lavender
        new Color(0.8f, 1f, 0.9f), // pastel mint
        new Color(0.9f, 1f, 0.8f), // pastel yellow-green
        new Color(1f, 0.95f, 0.8f), // pastel peach
        new Color(1f, 0.8f, 0.9f)
    };
}