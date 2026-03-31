using BaseLib.Config;

namespace GradientLine.GradientLineCode;

[HoverTipsByDefault]
public class Config: SimpleModConfig
{
    [SliderRange(0, 64, 1)]
    public static double Steps { get; set; } = 8;
    [SliderRange(30, 500, 10)]
    public static double GradientSpeed { get; set; } = 120f;
    
}