using BaseLib.Config;
using Godot;

namespace GradientLine.GradientLineCode;

[HoverTipsByDefault]
public class Config : SimpleModConfig
{
    public static GradientUtil.GradientType GradientType { get; set; } = GradientUtil.GradientType.Rainbow;
    public static bool Animate { get; set; } = true;
    public static bool RandomizeStartOffset { get; set; } = true;
    [SliderRange(30, 200, 10)]
    public static double AnimateSpeed { get; set; } = 120f;
    [ConfigTextInput("^((#[0-9A-Fa-f]{6})){1,10}$", MaxLength = 70)]
    
    public static string CustomColors { get; set; } = "";

    private readonly List<EventHandler> _configChangedHandlers = [];
    private float _previewHueOffset;
    private bool _wasRandomizeEnabled;
    private GradientUtil.GradientType _lastGradientType;

    
    public override void SetupConfigUI(Control optionContainer)
    {
        ClearUIEventHandlers();
        
        _wasRandomizeEnabled = RandomizeStartOffset;
        _lastGradientType = GradientType;
        _previewHueOffset = RandomizeStartOffset ? GD.Randf() : 0f;
        
        GenerateOptionsForAllProperties(optionContainer);
        AddGradientPreview(optionContainer);
        AddRestoreDefaultsButton(optionContainer);
    }

    private void AddGradientPreview(Control optionContainer)
    {
        GradientPreviewControl gradientPreview = new GradientPreviewControl();
        gradientPreview.CustomMinimumSize = new Vector2(120, 16);
        gradientPreview.SetGradient(GradientUtil.BuildGradient(_previewHueOffset));
        
        EventHandler gradientUpdateHandler = (sender, args) =>
        {
            if (!GodotObject.IsInstanceValid(gradientPreview))
                return;

            if (!gradientPreview.IsInsideTree())
                return;

            MaybeSometimesUpdatePreviewOffset();

            // Update tracking state
            _wasRandomizeEnabled = RandomizeStartOffset;
            _lastGradientType = GradientType;
            
            gradientPreview.SetGradient(GradientUtil.BuildGradient(_previewHueOffset));
        };
        
        ConfigChanged += gradientUpdateHandler;
        
        // Track this handler so it gets cleaned up
        _configChangedHandlers.Add(gradientUpdateHandler);
        
        optionContainer.AddChild(CreateSectionHeader("Preview"));
        optionContainer.AddChild(gradientPreview);
    }

    // This is gross so it's here.
    // Basically only update the preview if toggling between randomizing the starting hue or not
    // or when the gradient gets changed
    private void MaybeSometimesUpdatePreviewOffset()
    {
        // Check if randomize was just turned on
        bool randomizeJustEnabled = RandomizeStartOffset && !_wasRandomizeEnabled;
            
        // Check if gradient type changed
        bool gradientTypeChanged = GradientType != _lastGradientType;
            
        // Regenerate offset if randomize is on AND (it was just enabled OR gradient type changed)
        if (RandomizeStartOffset && (randomizeJustEnabled || gradientTypeChanged))
        {
            _previewHueOffset = GD.Randf();
        }
        // If randomize was turned off, reset to 0
        else if (!RandomizeStartOffset && _wasRandomizeEnabled)
        {
            _previewHueOffset = 0f;
        }
        // Otherwise keep the existing offset
    }

    private void ClearUIEventHandlers()
    {
        foreach (var handler in _configChangedHandlers)
            ConfigChanged -= handler;
        
        _configChangedHandlers.Clear();
    }
}