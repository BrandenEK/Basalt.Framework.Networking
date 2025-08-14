using System;

namespace Basalt.Framework.Networking.Serializers;

public interface IPacketSerializerLegacy
{
    public int PacketId { get; }

    public Type PacketType { get; }

    public object[] Serialize(BasePacket packet);

    public BasePacket Deserialize(string[] data);
}
