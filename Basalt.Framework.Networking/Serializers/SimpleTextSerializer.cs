using System;
using System.Collections.Generic;
using System.Text;

namespace Basalt.Framework.Networking.Serializers;

public class SimpleTextSerializer : ISerializer
{
    private readonly List<IPacketSerializer> _serializers = [];

    public SimpleTextSerializer()
    {
        _serializers.Add(new TextPacketSerializer());
    }

    public IEnumerable<BasePacket> Deserialize(byte[] data)
    {
        string text = Encoding.UTF8.GetString(data);

        List<BasePacket> packets = [];
        int start = 0, end;

        while (true)
        {
            end = text.IndexOf('/', start);

            if (end == -1)
                break;

            string part = text.Substring(start, end - start);
            start = end + 1;

            packets.Add(new TextPacket()
            {
                Text = part,
            });
        }

        return packets;
    }

    public byte[] Serialize(BasePacket packet)
    {
        if (packet is not TextPacket tpacket)
            throw new System.Exception("Only text packets");

        return Encoding.UTF8.GetBytes(tpacket.Text + '/');
    }
}

public class TextPacket : BasePacket
{
    public string Text { get; set; } = string.Empty;
}

public class TextPacketSerializer : IPacketSerializer
{
    public int PacketId { get; } = 0;

    public Type PacketType { get; } = typeof(TextPacket);

    public BasePacket Deserialize(string[] data)
    {
        return new TextPacket()
        {
            Text = data[0]
        };
    }

    public object[] Serialize(BasePacket packet)
    {
        TextPacket tpacket = (TextPacket)packet;

        return [tpacket.Text];
    }
}
