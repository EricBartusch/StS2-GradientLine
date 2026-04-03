using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Multiplayer.Transport;

namespace GradientLine.GradientLineCode.Networking;

public class LineStartMessage : INetMessage, IPacketSerializable
{
    public ulong PlayerId;
    public float StartingHue;

    public bool ShouldBroadcast => true;
    public NetTransferMode Mode => NetTransferMode.Reliable;
    public LogLevel LogLevel => LogLevel.Debug;

    public void Serialize(PacketWriter writer)
    {
        writer.WriteULong(PlayerId);
        writer.WriteFloat(StartingHue);
    }

    public void Deserialize(PacketReader reader)
    {
        PlayerId = reader.ReadULong();
        StartingHue = reader.ReadFloat();
    }
}