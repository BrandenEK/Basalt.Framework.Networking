using Basalt.Framework.Networking.Packets;
using System.Text;

namespace Basalt.Framework.Networking.PacketSerializers;

public class TextPacketSerializer : IPacketSerializer
{
    public byte[] Serialize(BasePacket packet)
    {
        TextPacket p = (TextPacket)packet;

        byte[] bytes = Encoding.UTF8.GetBytes(p.Text);
        return [(byte)bytes.Length, .. bytes];
    }

    public BasePacket Deserialize(byte[] data)
    {
        byte length = data[0];
        string text = Encoding.UTF8.GetString(data, 1, length);

        return new TextPacket()
        {
            Text = text
        };
    }
}
