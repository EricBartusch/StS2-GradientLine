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
            
            ulong netId = player.NetId;
            ulong localId = NetworkPatches.localPlayerId;
            bool isLocal = netId == localId;
            
            if (isLocal)
                __result.Gradient = GradientUtil.BuildGradient(NetworkPatches.LocalStartingHue);


            else
                __result.Gradient = GradientUtil.BuildSpecificGradient(NetworkPatches._playerGradients[netId],
                    NetworkPatches._playerStartingHues[netId]);
            
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
                ulong netId = Traverse.Create(state).Field("playerId").GetValue<ulong>();
                ulong localId = NetworkPatches.localPlayerId;
                bool isLocal = netId == localId;
                
                var line = Traverse.Create(state).Field("currentlyDrawingLine").GetValue<Line2D>();
                if (!GodotObject.IsInstanceValid(line) || line.Gradient == null) return;
            

                if (isLocal)
                {
                    float hueOffset = NetworkPatches.LocalStartingHue +
                                      (float)(line.GetPointCount() / Config.AnimateSpeed) % 1f;
                    line.Gradient = GradientUtil.BuildGradient(hueOffset);
                }

                else
                {
                    float hueOffset = NetworkPatches._playerStartingHues[netId] +
                                      (float)(line.GetPointCount() / Config.AnimateSpeed) % 1f;
                    line.Gradient = GradientUtil.BuildSpecificGradient(NetworkPatches._playerGradients[netId],
                        hueOffset);
                }

            }
        }
    }
}