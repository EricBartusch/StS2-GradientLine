using Godot;

namespace GradientLine.GradientLineCode;

public class GradientUtil
{
    public enum GradientType
    {
        Rainbow,
        Fire,
        Ocean,
        Monochrome,
        Christmas,
        Spring
    }

    public static Gradient BuildGradient(float hueOffset)
    {
        return Config.GradientType switch
        {
            GradientType.Fire => BuildKeyframeGradient(hueOffset, FireColors),
            GradientType.Ocean => BuildKeyframeGradient(hueOffset, OceanColors),
            GradientType.Monochrome => BuildKeyframeGradient(hueOffset, MonoColors),
            GradientType.Christmas => BuildKeyframeGradient(hueOffset, ChristmasColors),
            GradientType.Spring => BuildKeyframeGradient(hueOffset, SpringColors),
            GradientType.Rainbow => BuildRainbowGradient(hueOffset)
        };
    }

    static Gradient BuildKeyframeGradient(float hueOffset, Color[] keyColors)
    {
        const int Steps = 16;
        var colors = new Color[Steps];
        var offsets = new float[Steps];
        int n = keyColors.Length;

        for (int i = 0; i < Steps; i++)
        {
            offsets[i] = i / (float)(Steps - 1);

            // Position in the keyframe array, offset by hueOffset
            float t = ((offsets[i] + hueOffset) % 1f) * (n - 1);
            int lo = (int)t;
            int hi = Mathf.Min(lo + 1, n - 1);
            colors[i] = keyColors[lo].Lerp(keyColors[hi], t - lo);
        }

        return new Gradient { Colors = colors, Offsets = offsets };
    }

    static Gradient BuildRainbowGradient(float hueOffset)
    {
        const int Steps = 16;
        var colors = new Color[Steps];
        var offsets = new float[Steps];
        for (int i = 0; i < Steps; i++)
        {
            offsets[i] = i / (float)(Steps - 1);
            colors[i] = Color.FromHsv((hueOffset + offsets[i]) % 1f, 1f, 1f);
        }

        return new Gradient { Colors = colors, Offsets = offsets };
    }

    static readonly Color[] FireColors =
    [
        new Color(0.1f, 0f, 0f),
        new Color(0.8f, 0.1f, 0f),
        new Color(1f, 0.5f, 0f),
        new Color(1f, 0.9f, 0.2f),
        new Color(1f, 1f, 0.8f),
        new Color(0.1f, 0f, 0f)
    ];

    static readonly Color[] OceanColors =
    [
        new Color(0f, 0.05f, 0.2f),
        new Color(0f, 0.3f, 0.6f),
        new Color(0f, 0.7f, 0.9f),
        new Color(0.4f, 0.9f, 1f),
        new Color(0.8f, 1f, 1f),
        new Color(0f, 0.05f, 0.2f)
    ];

    static readonly Color[] MonoColors =
    [
        new Color(0.1f, 0.1f, 0.1f),
        new Color(1f, 1f, 1f),
        new Color(0.1f, 0.1f, 0.1f)
    ];
    
    static readonly Color[] ChristmasColors =
    [
        new Color(1f, 0f, 0f),
        new Color(1f, 1f, 1f),
        new Color(0f, 0.5f, 0f),
        new Color(0f, 0.5f, 0f),
        new Color(1f, 1f, 1f),
        new Color(1f, 0f, 0f)
    ];
    
    static readonly Color[] SpringColors =
    {
        new Color(1f, 0.8f, 0.9f), // pastel pink
        new Color(0.9f, 0.9f, 1f), // pastel lavender
        new Color(0.8f, 1f, 0.9f), // pastel mint
        new Color(0.9f, 1f, 0.8f), // pastel yellow-green
        new Color(1f, 0.95f, 0.8f), // pastel peach
        new Color(1f, 0.8f, 0.9f)
    };
}