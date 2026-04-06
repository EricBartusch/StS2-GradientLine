using Godot;

namespace GradientLine.GradientLineCode;

public class GradientUtil
{
    private const int Steps = 16;
    private static readonly Dictionary<GradientType, Color[]> BaseGradientColorsCache = new();

    public enum GradientType : ushort
    {
        Rainbow,
        Fire,
        Ocean,
        Monochrome,
        Christmas,
        Spring,
        Random,
        Custom,
        None
    }

    public static Gradient BuildGradient(GradientType type, float hueOffset, Gradient? savedRandomGradient = null, bool reRandomize = false)
    {
        if (type == GradientType.Random)
        {
            if (savedRandomGradient is null || reRandomize)
            {
                // Generate new random - don't cache it since it should be ephemeral
                return BuildKeyframeGradient(hueOffset, BuildRandomColors((int)Config.RandomGradientSize));
            }
            return BuildKeyframeFromGradientColors(savedRandomGradient, hueOffset);
        }
    
        // Cache these values as used because they will never change
        if (!BaseGradientColorsCache.TryGetValue(type, out var baseColors))
        {
            baseColors = type switch
            {
                GradientType.Fire => GradientsPresets.FireColors,
                GradientType.Ocean => GradientsPresets.OceanColors,
                GradientType.Monochrome => GradientsPresets.MonoColors,
                GradientType.Christmas => GradientsPresets.ChristmasColors,
                GradientType.Spring => GradientsPresets.SpringColors,
                GradientType.Rainbow => null, // Handled differently
                GradientType.Custom => null,  // same
                _ => null
            };
        
            if (baseColors != null)
                BaseGradientColorsCache[type] = baseColors;
        }
    
        if (type == GradientType.Rainbow)
            return BuildRainbowGradient(hueOffset);
    
        if (type == GradientType.Custom)
            return BuildKeyframeCustomGradient(hueOffset);
    
        return BuildKeyframeGradient(hueOffset, baseColors);
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
    
    public static Gradient BuildKeyframeFromGradientColors(Gradient gradient, float hueOffset)
    {
        return BuildKeyframeGradient(hueOffset, gradient.Colors);
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

        float maxDelta = (float)Config.Randomness;

        if (maxDelta != 1f)
        {
            for (int i = 1; i < size; i++)
            {
                Color prev = colors[i - 1];
                colors[i] = new Color(
                    Mathf.Clamp(prev.R + ((float)rng.NextDouble() - 0.5f) * 2 * maxDelta, 0f, 1f),
                    Mathf.Clamp(prev.G + ((float)rng.NextDouble() - 0.5f) * 2 * maxDelta, 0f, 1f),
                    Mathf.Clamp(prev.B + ((float)rng.NextDouble() - 0.5f) * 2 * maxDelta, 0f, 1f)
                );
            }
        }

        colors[size] = colors[0]; // Make last the same as the first so it looks nicer
        return colors;
    }
}