
namespace Basalt.Framework.Networking.PacketSerializers;

public interface IPacketSerializer
{
    public byte[] Serialize(BasePacket packet);

    public BasePacket Deserialize(byte[] data);
}
