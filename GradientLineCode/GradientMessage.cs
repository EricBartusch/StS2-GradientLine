using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Multiplayer.Transport;

namespace GradientLine.GradientLineCode;

public class GradientMessage : INetMessage, IPacketSerializable
{
    public ulong PlayerId;
    public GradientUtil.GradientType GradientType;
    public float StartingHue;

    public bool ShouldBroadcast => true;
    public NetTransferMode Mode => NetTransferMode.Reliable;
    public LogLevel LogLevel => LogLevel.Info;

    public void Serialize(PacketWriter writer)
    {
        writer.WriteULong(PlayerId);
        writer.WriteUShort((ushort)GradientType);
        writer.WriteFloat(StartingHue);
    }

    public void Deserialize(PacketReader reader)
    {
        PlayerId = reader.ReadULong();
        GradientType = (GradientUtil.GradientType)reader.ReadShort();
        StartingHue = reader.ReadFloat();
    }
    
}