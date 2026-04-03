using Godot;
using GodotPlugins.Game;
using GradientLine.GradientLineCode.Networking;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;

namespace GradientLine.GradientLineCode;

public class GradientLinePatches
{
    [HarmonyPatch(typeof(NMapDrawings), "CreateLineForPlayer")]
    public static class SetGradientPatch
    {
        [HarmonyPostfix]
        static void Postfix(NMapDrawings __instance, Player player, bool isErasing, ref Line2D __result)
        {
            if (__result == null || isErasing) return;
            
            ulong playerId = player.NetId;
            float startingHue = Config.RandomizeStartOffset ? GD.Randf() : 0f;
            
            if (MultiplayerManager.IsLocalPlayer(playerId))
            {
                __result.Gradient = GradientUtil.BuildGradient(startingHue);
                MultiplayerManager.BroadcastLineStart(startingHue);
            }
            else
            {
                float remoteHue = MultiplayerManager.GetCurrentLineHue(playerId);
                __result.Gradient = GradientUtil.BuildSpecificGradient(
                    MultiplayerManager.GetPlayerGradientType(playerId),
                    remoteHue);
            }
        }
    }

    [HarmonyPatch(typeof(NMapDrawings), "UpdateCurrentLinePosition")]
    public static class UpdateGradientPatch
    {
        [HarmonyPostfix]
        static void Postfix(NMapDrawings __instance, object state, Vector2 position)
        {
            if (!Config.Animate)
                return;
            
            ulong netId = Traverse.Create(state).Field("playerId").GetValue<ulong>();
            var line = Traverse.Create(state).Field("currentlyDrawingLine").GetValue<Line2D>();
            if (!GodotObject.IsInstanceValid(line) || line.Gradient == null) return;

            float currentLineHue = MultiplayerManager.GetCurrentLineHue(netId);
            float hueOffset = currentLineHue + (float)(line.GetPointCount() / Config.AnimateSpeed) % 1f;

            if (MultiplayerManager.IsLocalPlayer(netId))
            {
                line.Gradient = GradientUtil.BuildGradient(hueOffset);
            }
            else
            {
                line.Gradient = GradientUtil.BuildSpecificGradient(
                    MultiplayerManager.GetPlayerGradientType(netId),
                    hueOffset);
            }
        }
    }
}