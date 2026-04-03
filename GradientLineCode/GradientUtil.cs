using Godot;

namespace GradientLine.GradientLineCode;

public class GradientUtil
{

    private const int Steps = 16;
    public static Gradient? CreatedGradient;
    public enum GradientType : ushort
    {
        Rainbow,
        Fire,
        Ocean,
        Monochrome,
        Christmas,
        Spring,
        Random,
        Custom
    }

    public static Gradient BuildGradientFromConfig(float hueOffset)
    {
        return Config.GradientType switch
        {
            GradientType.Fire => BuildKeyframeGradient(hueOffset, GradientsPresets.FireColors),
            GradientType.Ocean => BuildKeyframeGradient(hueOffset, GradientsPresets.OceanColors),
            GradientType.Monochrome => BuildKeyframeGradient(hueOffset, GradientsPresets.MonoColors),
            GradientType.Christmas => BuildKeyframeGradient(hueOffset, GradientsPresets.ChristmasColors),
            GradientType.Spring => BuildKeyframeGradient(hueOffset, GradientsPresets.SpringColors),
            GradientType.Random => BuildKeyframeGradient(0f, BuildRandomColors((int)Config.RandomGradientSize)),
            GradientType.Custom => BuildKeyframeCustomGradient(hueOffset),
            GradientType.Rainbow => BuildRainbowGradient(hueOffset)
        };
    }
    
    public static Gradient BuildSpecificGradient(GradientType type, float hueOffset)
    {
        return type switch
        {
            GradientType.Fire => BuildKeyframeGradient(hueOffset, GradientsPresets.FireColors),
            GradientType.Ocean => BuildKeyframeGradient(hueOffset, GradientsPresets.OceanColors),
            GradientType.Monochrome => BuildKeyframeGradient(hueOffset, GradientsPresets.MonoColors),
            GradientType.Christmas => BuildKeyframeGradient(hueOffset, GradientsPresets.ChristmasColors),
            GradientType.Spring => BuildKeyframeGradient(hueOffset, GradientsPresets.SpringColors),
            GradientType.Random => BuildKeyframeGradient(0f, BuildRandomColors((int)Config.RandomGradientSize)),
            GradientType.Custom => BuildKeyframeCustomGradient(hueOffset),
            GradientType.Rainbow => BuildRainbowGradient(hueOffset)
        };
    }

    private static Gradient BuildKeyframeGradient(float hueOffset, Color[] keyColors)
    {
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

    private static Gradient BuildKeyframeCustomGradient(float hueOffset)
    {
        string hexString = Config.CustomColors;

        string[] parts = hexString.Split('#', StringSplitOptions.RemoveEmptyEntries);
        Color[] colors = parts
            .Select(hex => new Color($"#{hex}"))
            .ToArray();
        
        return BuildKeyframeGradient(hueOffset, colors);
    }

    private static Gradient BuildRainbowGradient(float hueOffset)
    {
        var colors = new Color[Steps];
        var offsets = new float[Steps];
        for (int i = 0; i < Steps; i++)
        {
            offsets[i] = i / (float)(Steps - 1);
            colors[i] = Color.FromHsv((hueOffset + offsets[i]) % 1f, 1f, 1f);
        }

        return new Gradient { Colors = colors, Offsets = offsets };
    }
    
    private static Color[] BuildRandomColors(int size)
    {
        var rng = new Random();
        var colors = new Color[size + 1]; // account for making the smoothing color
        
        for (int i = 0; i < size; i++)
        {
            colors[i] = new Color(
                (float)rng.NextDouble(),
                (float)rng.NextDouble(),
                (float)rng.NextDouble()
            );
        }
        
        colors[size] = colors[0]; // Make last the same as the first so it looks nicer
        return colors;
    }
}