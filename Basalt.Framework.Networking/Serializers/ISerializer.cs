
namespace Basalt.Framework.Networking.Serializers;

public interface ISerializer
{
    public byte[] Serialize(BasePacket packet);

    public BasePacket Deserialize(byte[] data);
}
