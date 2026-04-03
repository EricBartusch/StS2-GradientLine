using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;

namespace GradientLine.GradientLineCode;

[HarmonyPatch]
public static class NetworkPatches
{
    public static ulong localPlayerId;
    private static INetGameService? _netGameService;
    public static readonly Dictionary<ulong, GradientUtil.GradientType> _playerGradients = new();
    public static readonly Dictionary<ulong, float> _playerStartingHues = new();
    public static float LocalStartingHue = GD.Randf();
    
    static void BroadcastGradient()
    {
        if (localPlayerId == 0) return;
        _netGameService.SendMessage(
            new GradientMessage {PlayerId = localPlayerId, GradientType = Config.GradientType, StartingHue = LocalStartingHue});
    }
    
    [HarmonyPatch(typeof(NCharacterSelectScreen), "PlayerConnected")]
    [HarmonyPostfix]
    static void OnPlayerConnected(LobbyPlayer player)
    {
        if (player.id == localPlayerId) return;
        BroadcastGradient();
    }
    
    [HarmonyPatch(typeof(NMapScreen), "Initialize")]
    [HarmonyPostfix]
    static void MapScreenInitialize(RunState runState)
    {
        BroadcastGradient();
    }
    
    [HarmonyPatch(typeof(StartRunLobby), MethodType.Constructor,
        new[] { typeof(GameMode), typeof(INetGameService), typeof(IStartRunLobbyListener), typeof(int) })]
    [HarmonyPostfix]
    static void LobbyConstructed(StartRunLobby __instance)
    {
        SetupNetService(__instance.NetService, __instance);
    }

    [HarmonyPatch(typeof(LoadRunLobby), MethodType.Constructor,
        new[] { typeof(INetGameService), typeof(ILoadRunLobbyListener), typeof(SerializableRun) })]
    [HarmonyPostfix]
    static void LoadRunLobbyConstructed(LoadRunLobby __instance)
    {
        SetupNetService(__instance.NetService, __instance);
    }

    static void SetupNetService(INetGameService netService, object lobby)
    {
        _netGameService = netService;
        localPlayerId = netService.NetId;
        netService.RegisterMessageHandler<GradientMessage>(RecieveGradient);
    }
    
    static void RecieveGradient(GradientMessage message, ulong senderId)
    {
        _playerGradients.Add(senderId, message.GradientType);
        _playerStartingHues.Add(senderId, message.StartingHue);
    }
}
