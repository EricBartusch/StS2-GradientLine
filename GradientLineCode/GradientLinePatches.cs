using BaseLib.Utils;
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
            GradientFields.StartingHue.Set(__instance, GD.Randf());
            __result.Gradient = GradientUtil.BuildGradient(GradientFields.StartingHue.Get(__instance));
        }
    }

    [HarmonyPatch(typeof(NMapDrawings),  "UpdateCurrentLinePosition")]
    public static class AdvanceRainbowPatch
    {

        [HarmonyPostfix]
        static void Postfix(NMapDrawings __instance, object state, Vector2 position)
        {
            if (Config.Animate)
            {
                var line = Traverse.Create(state).Field("currentlyDrawingLine").GetValue<Line2D>();
                if (!GodotObject.IsInstanceValid(line) || line.Gradient == null) return;

                float hueOffset = GradientFields.StartingHue.Get(__instance) +
                                  ((float)((line.GetPointCount() / Config.AnimateSpeed)) % 1f);
                line.Gradient = GradientUtil.BuildGradient(hueOffset);
            }
        }
    }
}

public class GradientFields
{
    public static readonly SpireField<NMapDrawings, float> StartingHue = new(() => 0f);
}