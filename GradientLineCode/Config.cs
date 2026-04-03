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

    [SliderRange(2, 10)]
    public static double RandomGradientSize { get; set; } = 5;

    private readonly List<EventHandler> _configChangedHandlers = [];
    private float _previewHueOffset;
    private bool _wasRandomizeEnabled;
    private double _lastRandomGradientSize;
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
        
        // Save the gradient made here for if they use the random option
        GradientUtil.CreatedGradient = GradientUtil.BuildGradientFromConfig(_previewHueOffset);
        gradientPreview.SetGradient(GradientUtil.CreatedGradient);
        
        EventHandler gradientUpdateHandler = (sender, args) =>
        {
            if (!GodotObject.IsInstanceValid(gradientPreview))
                return;

            if (!gradientPreview.IsInsideTree())
                return;

            bool shouldRebuildGradient = ShouldRebuildGradient();

            MaybeSometimesUpdatePreviewOffset();

            // Update tracking state
            _wasRandomizeEnabled = RandomizeStartOffset;
            bool reselectedRandom =
                GradientType == GradientUtil.GradientType.Random &&
                _lastGradientType == GradientUtil.GradientType.Random;
            _lastGradientType = GradientType;
            _lastRandomGradientSize = RandomGradientSize;

            if (shouldRebuildGradient || reselectedRandom)
            {
                GradientUtil.CreatedGradient = GradientUtil.BuildGradientFromConfig(_previewHueOffset);
                gradientPreview.SetGradient(GradientUtil.CreatedGradient);
            }
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
    
    // Random gradient should only be regenerated if the gradient or the amount of colors in the gradient changed
    private bool ShouldRebuildGradient()
    {
        bool gradientTypeChanged = GradientType != _lastGradientType;
        bool customColorsChanged = RandomGradientSize != _lastRandomGradientSize;

        return gradientTypeChanged || customColorsChanged;
    }

    private void ClearUIEventHandlers()
    {
        foreach (var handler in _configChangedHandlers)
            ConfigChanged -= handler;
        
        _configChangedHandlers.Clear();
    }
}