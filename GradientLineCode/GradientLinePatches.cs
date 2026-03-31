using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;

namespace GradientLine.GradientLineCode;

public class GradientLinePatches
{
    [HarmonyPatch(typeof(NMapDrawings),  "CreateLineForPlayer")]
    public static class SetRainbowGradientPatch
    {
        [HarmonyPostfix]
        static void Postfix(NMapDrawings __instance, Player player, bool isErasing, ref Line2D __result)
        {
            if (__result == null || isErasing) return;
            __result.Gradient = BuildRainbowGradient(0f);
        }
    }

    [HarmonyPatch(typeof(NMapDrawings),  "UpdateCurrentLinePosition")]
    public static class AdvanceRainbowPatch
    {

        [HarmonyPostfix]
        static void Postfix(NMapDrawings __instance, object state, Vector2 position)
        {
            var line = Traverse.Create(state).Field("currentlyDrawingLine").GetValue<Line2D>();
            if (!GodotObject.IsInstanceValid(line) || line.Gradient == null) return;

            float hueOffset = (line.GetPointCount() / 120f) % 1f;
            line.Gradient = BuildRainbowGradient(hueOffset);
        }
    }
    
    static Gradient BuildRainbowGradient(float hueOffset)
    {
        const int Steps = 8;
        var colors = new Color[Steps];
        var offsets = new float[Steps];

        for (int i = 0; i < Steps; i++)
        {
            offsets[i] = i / (float)(Steps - 1);
            colors[i] = Color.FromHsv((hueOffset + offsets[i]) % 1f, 1f, 1f);
        }

        return new Gradient { Colors = colors, Offsets = offsets };
    }
}