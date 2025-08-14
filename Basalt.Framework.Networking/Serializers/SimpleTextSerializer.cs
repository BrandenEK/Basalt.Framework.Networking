using Basalt.Framework.Networking.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basalt.Framework.Networking.Serializers;

public class SimpleTextSerializer : ISerializer
{
    private readonly List<IPacketSerializer> _serializers = [];

    public SimpleTextSerializer()
    {
        _serializers.Add(new TextPacketSerializer());
        _serializers.Add(new TestDataPacketSerializer());
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

            string[] parts = part.Split(':');
            int id = int.Parse(parts[0]);

            IPacketSerializer? serializer = _serializers.FirstOrDefault(x => x.PacketId == id);

            if (serializer == null)
                throw new NetworkDataException($"Can not serialize packet id {id}");

            BasePacket packet = serializer.Deserialize(parts[1..]);
            packets.Add(packet);
        }

        return packets;
    }

    public byte[] Serialize(BasePacket packet)
    {
        IPacketSerializer? serializer = _serializers.FirstOrDefault(x => x.PacketType == packet.GetType());

        if (serializer == null)
            throw new NetworkDataException($"Can not serialize packet type {packet.GetType().Name}");

        object[] data = serializer.Serialize(packet);
        string text = $"{serializer.PacketId}:{string.Join(':', data)}/";

        return Encoding.UTF8.GetBytes(text);
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

    public class TestDataPacketSerializer : IPacketSerializer
    {
        public int PacketId { get; } = 9;

        public Type PacketType { get; } = typeof(TestDataPacket);

        public BasePacket Deserialize(string[] data)
        {
            return new TestDataPacket()
            {
                Name = data[0],
                Points = int.Parse(data[1]),
                TimeStamp = new DateTime(long.Parse(data[2])),
            };
        }

        public object[] Serialize(BasePacket packet)
        {
            TestDataPacket tpacket = (TestDataPacket)packet;

            return
            [
                tpacket.Name,
                tpacket.Points,
                tpacket.TimeStamp.Ticks,
            ];
        }
    }
}
