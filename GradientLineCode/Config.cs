using BaseLib.Config;

namespace GradientLine.GradientLineCode;

[HoverTipsByDefault]
public class Config: SimpleModConfig
{
    public static bool Animate { get; set; } = true;
    [SliderRange(30, 500, 10)]
    public static double AnimateSpeed { get; set; } = 120f;
}