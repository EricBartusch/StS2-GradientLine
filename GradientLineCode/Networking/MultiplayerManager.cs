using Godot;
using MegaCrit.Sts2.Core.Multiplayer.Game;

namespace GradientLine.GradientLineCode.Networking;

public static class MultiplayerManager
{
    private static INetGameService? _netGameService;
    private static ulong _localPlayerId;
    
    private static readonly Dictionary<ulong, GradientUtil.GradientType> _playerGradientTypes = new();
    private static readonly Dictionary<ulong, float> _currentLineHues = new();
    private static readonly Dictionary<ulong, Gradient> _playerGradients = new(); // currently only used for sending generated random gradients
    
    public static ulong LocalPlayerId => _localPlayerId;
    
    public static void Initialize(INetGameService netGameService)
    {
        _netGameService = netGameService;
        _localPlayerId = netGameService.NetId;
        // They're all prefixed with ZZ to put them at the end of the Messages list in order to allow the game to start with a nonmodded client
        netGameService.RegisterMessageHandler<ZZGradientTypeMessage>(OnGradientMessageReceived);
        netGameService.RegisterMessageHandler<ZZLineStartMessage>(OnLineStartMessageReceived);
        netGameService.RegisterMessageHandler<ZZGradientMessage>(OnGradientMessageReceived);
    }
    
    public static void BroadcastGradientType()
    {
        if (_localPlayerId == 0 || _netGameService == null)
        {
            return;
        }
        
        var message = new ZZGradientTypeMessage
        {
            PlayerId = _localPlayerId,
            GradientType = Config.GradientType
        };
        
        _netGameService.SendMessage(message);
    }

    public static void BroadcastGradient()
    {
        if (_localPlayerId == 0 || _netGameService == null)
        {
            return;
        }

        if (Config.GetSavedRandomGradient() is null)
        {
            Config.SetSavedRandomGradient(GradientUtil.BuildGradient(GradientUtil.GradientType.Random, Config.GetPreviewHueOffset()));
        }

        var message = new ZZGradientMessage()
        {
            PlayerId = _localPlayerId,
            Colors = Config.GetSavedRandomGradient()?.Colors,
            Offsets = Config.GetSavedRandomGradient()?.Offsets
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
        var message = new ZZLineStartMessage
        {
            PlayerId = _localPlayerId,
            StartingHue = startingHue
        };
        
        _netGameService.SendMessage(message);
    }

    public static GradientUtil.GradientType GetPlayerGradientType(ulong playerId)
    {
        return _playerGradientTypes.TryGetValue(playerId, out var type) 
            ? type 
            : GradientUtil.GradientType.None;
    }
    
    public static float GetCurrentLineHue(ulong playerId)
    {
        return _currentLineHues.TryGetValue(playerId, out var hue) 
            ? hue 
            : 0f;
    }
    
    public static Gradient GetPlayerGradient(ulong playerId)
    {
        return _playerGradients.TryGetValue(playerId, out var type) 
            ? type 
            : GradientUtil.BuildGradient(GradientUtil.GradientType.Rainbow, 0f);
    }
    
    public static bool IsLocalPlayer(ulong playerId)
    {
        if (_netGameService == null)
            return true;
        return playerId == _localPlayerId;
    }
    
    private static void OnGradientMessageReceived(ZZGradientTypeMessage typeMessage, ulong senderId)
    {
        _playerGradientTypes[senderId] = typeMessage.GradientType;
    }
    
    private static void OnLineStartMessageReceived(ZZLineStartMessage message, ulong senderId)
    {
        _currentLineHues[senderId] = message.StartingHue;
    }
    
    private static void OnGradientMessageReceived(ZZGradientMessage message, ulong senderId)
    {
        _playerGradients[senderId] = message.ToGradient();
    }
}