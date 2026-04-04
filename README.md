# Map Line Gradients

A mod for **Slay the Spire 2** that replaces the plain-colored freehand map drawing lines with animated color gradients.

## Features

- **Built-in gradient presets**
- **Custom palette** - define your own gradient using up to 10 hex color codes
- **Live preview** of the selected gradient in the mod settings UI
- **Multiplayer support** - each player's gradient preferences are synced across clients
- **Random gradient** - can be random each line drawn or the same pregenerated random gradient

## Requirements

- Slay the Spire 2 (Early Access)
- [BaseLib](https://github.com/alchyr/BaseLib) mod installed

## Installation

1. Install BaseLib if you haven't already.
2. Copy the `GradientLine/` folder (containing `GradientLine.dll`, `GradientLine.pck`, and `GradientLine.json`) into your StS2 mods directory.

## Configuration

All settings are available in-game through the mod settings menu:

| Setting | Default | Description |
|---|---|---|
| **Gradient Type** | Rainbow | Choose from: Rainbow, Fire, Ocean, Monochrome, Christmas, Spring, Random, or Custom |
| **Animate** | On | Whether the gradient shifts/animates as you draw |
| **Randomize Start Offset** | On | Whether each line starts with a random position in the gradient |
| **Animation Speed** | 120 | Controls how fast the gradient shifts while drawing (range: 30–200, higher = faster) |

### Custom Gradient Settings
*(Only visible when Gradient Type is set to "Custom")*

| Setting | Default | Description |
|---|---|---|
| **Custom Colors** | _(empty)_ | Up to 10 hex color codes, each prefixed with `#` (e.g. `#FF0000#00FF00#0000FF`) |

### Random Gradient Settings
*(Only visible when Gradient Type is set to "Random")*

| Setting | Default | Description |
|---|---|---|
| **Random Gradient Size** | 5 | Number of color keyframes in the random gradient (range: 2–10) |
| **Randomize Each Line** | Off | Generate a new random gradient for every line drawn |
| **Randomness** | 1.0 | Controls color smoothness (range: 0.1–1.0). Lower values = smoother transitions between colors, higher values = more chaotic/vibrant |
| **Reroll Random** | _(button)_ | Generate a new random gradient immediately |

A live gradient preview strip updates in real time as you change settings.

## Building from Source

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- Godot 4.5.1 (the version bundled with StS2 / "Megadot") - needed only for exporting the `.pck`
- Slay the Spire 2 installed via Steam

The project auto-detects the Steam library location on Windows, Linux, and macOS. If it can't find the game, the build will error with a helpful message.

### Build (DLL only)

```sh
dotnet build
```

This compiles the mod and copies `GradientLine.dll`, `GradientLine.json`, and required BaseLib files directly into the StS2 mods directory.

### Publish (DLL + PCK)

```sh
dotnet publish
```

This additionally exports the Godot `.pck` asset bundle using Godot in headless mode.


## Contributing New Gradient Presets

Adding a new built-in preset requires changes to two files:

### 1. Add the preset to `GradientType` enum in [GradientUtil.cs](GradientLineCode/GradientUtil.cs)

```csharp
public enum GradientType : ushort
{
    Rainbow,
    Fire,
    // ...
    YourPreset, // add here (before Custom)
    Random,  
    Custom
}
```

### 2. Define color keyframes in [GradientsPresets.cs](GradientLineCode/GradientsPresets.cs)

Making the last color identical to the first is recommended because a mismatch between the end and start colors creates a visible hard

### 3. Wire it up in `BuildGradient`

Add a case to the switch expression:

```csharp
public static Gradient BuildGradient(float hueOffset)
{
    return Config.GradientType switch
    {
        // ...
        GradientType.Christmas => GradientsPresets.ChristmasColors,
        GradientType.Spring => GradientsPresets.SpringColors,
        GradientType.YourPreset => GradientsPresets.YourPresetColors,
        GradientType.Rainbow => null, // Handled differently
        GradientType.Custom => null,  // same
        _ => null
    };
}
```

## TODO
- Color picker for custom colors
- Make erasing better
- Allow gradient to repeat
