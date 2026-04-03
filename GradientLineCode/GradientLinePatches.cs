using Godot;
using GodotPlugins.Game;
using GradientLine.GradientLineCode.Networking;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;

namespace GradientLine.GradientLineCode;

public class GradientLinePatches
{
    [HarmonyPatch(typeof(NMapDrawings),  "CreateLineForPlayer")]
    public static class SetGradientPatch
    {
        [HarmonyPostfix]
        static void Postfix(NMapDrawings __instance, Player player, bool isErasing, ref Line2D __result)
        {
            if (__result == null || isErasing) return;
            
            if (MultiplayerManager.IsLocalPlayer(player.NetId))
                __result.Gradient = GradientUtil.BuildGradient(MultiplayerManager.LocalStartingHue);
            
            else
                __result.Gradient = GradientUtil.BuildSpecificGradient(MultiplayerManager.GetPlayerGradientType(player.NetId),
                    MultiplayerManager.GetPlayerStartingHue(player.NetId));
            
        }
    }

    [HarmonyPatch(typeof(NMapDrawings),  "UpdateCurrentLinePosition")]
    public static class UpdateGradientPatch
    {
        [HarmonyPostfix]
        static void Postfix(NMapDrawings __instance, object state, Vector2 position)
        {
            
            if (Config.Animate)
            {
                ulong netId = Traverse.Create(state).Field("playerId").GetValue<ulong>();
                var line = Traverse.Create(state).Field("currentlyDrawingLine").GetValue<Line2D>();
                if (!GodotObject.IsInstanceValid(line) || line.Gradient == null) return;

                if (MultiplayerManager.IsLocalPlayer(netId))
                {
                    float hueOffset = MultiplayerManager.LocalStartingHue +
                                      (float)(line.GetPointCount() / Config.AnimateSpeed) % 1f;
                    line.Gradient = GradientUtil.BuildGradient(hueOffset);
                }

                else
                {
                    float hueOffset = MultiplayerManager.GetPlayerStartingHue(netId) +
                                      (float)(line.GetPointCount() / Config.AnimateSpeed) % 1f;
                    line.Gradient = GradientUtil.BuildSpecificGradient(MultiplayerManager.GetPlayerGradientType(netId),
                        hueOffset);
                }
            }
        }
    }
}