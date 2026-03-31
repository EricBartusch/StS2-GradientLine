using BaseLib.Config;

namespace GradientLine.GradientLineCode;

[HoverTipsByDefault]
public class Config: SimpleModConfig
{
    public static GradientUtil.GradientType GradientType { get; set; } = GradientUtil.GradientType.Rainbow;
    public static bool Animate { get; set; } = true;
    [SliderRange(30, 200, 10)]
    public static double AnimateSpeed { get; set; } = 120f;

}