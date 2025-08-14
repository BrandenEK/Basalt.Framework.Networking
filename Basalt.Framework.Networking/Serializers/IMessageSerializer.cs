using System.Collections.Generic;

namespace Basalt.Framework.Networking.Serializers;

public interface IMessageSerializer
{
    public byte[] Serialize(BasePacket packet);

    public IEnumerable<BasePacket> Deserialize(byte[] data);
}
