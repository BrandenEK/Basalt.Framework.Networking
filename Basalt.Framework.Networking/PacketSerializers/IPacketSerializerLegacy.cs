using System;

namespace Basalt.Framework.Networking.PacketSerializers;

public interface IPacketSerializerLegacy
{
    public int PacketId { get; }

    public Type PacketType { get; }

    public object[] Serialize(BasePacket packet);

    public BasePacket Deserialize(string[] data);
}
