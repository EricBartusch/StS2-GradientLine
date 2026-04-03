using Godot;
using MegaCrit.Sts2.Core.Multiplayer.Game;

namespace GradientLine.GradientLineCode.Networking;

public static class MultiplayerManager
{
    private static INetGameService? _netGameService;
    private static ulong _localPlayerId;
    
    private static readonly Dictionary<ulong, GradientUtil.GradientType> _playerGradients = new();
    private static readonly Dictionary<ulong, float> _currentLineHues = new();
    
    public static ulong LocalPlayerId => _localPlayerId;
    
    public static void Initialize(INetGameService netGameService)
    {
        _netGameService = netGameService;
        _localPlayerId = netGameService.NetId;
        netGameService.RegisterMessageHandler<GradientMessage>(OnGradientMessageReceived);
        netGameService.RegisterMessageHandler<LineStartMessage>(OnLineStartMessageReceived);
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
            GradientType = Config.GradientType
        };
        
        _netGameService.SendMessage(message);
    }

    public static void BroadcastLineStart(float startingHue)
    {
        if (_localPlayerId == 0 || _netGameService == null)
            return;
        
        // Store locally
        _currentLineHues[_localPlayerId] = startingHue;
        
        // Broadcast to other players
        var message = new LineStartMessage
        {
            PlayerId = _localPlayerId,
            StartingHue = startingHue
        };
        
        _netGameService.SendMessage(message);
    }

    public static GradientUtil.GradientType GetPlayerGradientType(ulong playerId)
    {
        return _playerGradients.TryGetValue(playerId, out var type) 
            ? type 
            : GradientUtil.GradientType.Rainbow;
    }
    
    public static float GetCurrentLineHue(ulong playerId)
    {
        return _currentLineHues.TryGetValue(playerId, out var hue) 
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
    }
    
    private static void OnLineStartMessageReceived(LineStartMessage message, ulong senderId)
    {
        _currentLineHues[senderId] = message.StartingHue;
    }
}