using BaseLib.Config;
using BaseLib.Config.UI;
using Godot;

namespace GradientLine.GradientLineCode;

[HoverTipsByDefault]
public partial class Config : SimpleModConfig
{
    public static GradientUtil.GradientType GradientType { get; set; } = GradientUtil.GradientType.Rainbow;
    public static bool Animate { get; set; } = true;
    [SliderRange(30, 200, 10)]
    public static double AnimateSpeed { get; set; } = 120f;
    [ConfigTextInput("^((#[0-9A-Fa-f]{6})){1,10}$", MaxLength = 70)]
    public static string CustomColors { get; set; } = "";

    
    public override void SetupConfigUI(Control optionContainer)
    {
        GenerateOptionsForAllProperties(optionContainer);
        AddGradientPreview(optionContainer);
        AddRestoreDefaultsButton(optionContainer);
        
    }
    
    protected void AddGradientPreview(Control optionContainer)
    {
        GradientPreviewControl gradientPreview = new GradientPreviewControl();
        gradientPreview.CustomMinimumSize = new Vector2(120, 16);
        gradientPreview.SetGradient(GradientUtil.BuildGradient(0f));
        
        ConfigChanged += (sender, args) =>
        {
            if (gradientPreview == null)
                return;

            if (!GodotObject.IsInstanceValid(gradientPreview))
                return;

            if (!gradientPreview.IsInsideTree())
                return;


            gradientPreview.SetGradient(
                GradientUtil.BuildGradient(0f)
            );
        };
        
        optionContainer.AddChild(CreateSectionHeader("Preview"));
        optionContainer.AddChild(gradientPreview);

    }
}