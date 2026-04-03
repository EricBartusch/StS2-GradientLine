using Godot;
using MegaCrit.Sts2.Core.Multiplayer.Game;

namespace GradientLine.GradientLineCode.Networking;

public static class MultiplayerManager
{
    private static INetGameService? _netGameService;
    private static ulong _localPlayerId;
    
    private static readonly Dictionary<ulong, GradientUtil.GradientType> _playerGradients = new();
    private static readonly Dictionary<ulong, float> _playerStartingHues = new();

    
    public static ulong LocalPlayerId => _localPlayerId;
    public static float LocalStartingHue { get; private set; } = Config.RandomizeStartOffset ? GD.Randf() : 0f;
    
    public static void Initialize(INetGameService netGameService)
    {
        _netGameService = netGameService;
        _localPlayerId = netGameService.NetId;
        netGameService.RegisterMessageHandler<GradientMessage>(OnGradientMessageReceived);
    }
    
    public static void BroadcastLocalGradient()
    {
        if (_localPlayerId == 0 || _netGameService == null)
        {
            return;
        }
        
        var message = new GradientMessage
        {
            PlayerId = _localPlayerId,
            GradientType = Config.GradientType,
            StartingHue = LocalStartingHue
        };
        
        _netGameService.SendMessage(message);
    }

    public static GradientUtil.GradientType GetPlayerGradientType(ulong playerId)
    {
        return _playerGradients.TryGetValue(playerId, out var type) 
            ? type 
            : GradientUtil.GradientType.Rainbow;
    }

    public static float GetPlayerStartingHue(ulong playerId)
    {
        return _playerStartingHues.TryGetValue(playerId, out var hue) 
            ? hue 
            : 0f;
    }
    
    public static bool IsLocalPlayer(ulong playerId)
    {
        if (_netGameService == null)
            return true;
        return playerId == _localPlayerId;
    }
    
    private static void OnGradientMessageReceived(GradientMessage message, ulong senderId)
    {
        _playerGradients[senderId] = message.GradientType;
        _playerStartingHues[senderId] = message.StartingHue;
    }
}