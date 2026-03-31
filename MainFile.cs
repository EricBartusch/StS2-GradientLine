using BaseLib.Config;
using Godot;
using GradientLine.GradientLineCode;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace GradientLine;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "GradientLine";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        ModConfigRegistry.Register(ModId, new Config());

        Harmony harmony = new(ModId);

        harmony.PatchAll();
    }
}