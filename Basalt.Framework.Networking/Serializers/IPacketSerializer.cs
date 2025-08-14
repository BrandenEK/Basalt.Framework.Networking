
namespace Basalt.Framework.Networking.Serializers;

public interface IPacketSerializer
{
    public byte[] Serialize(BasePacket packet);

    public BasePacket Deserialize(byte[] data);
}
