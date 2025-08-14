using Basalt.Framework.Networking.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basalt.Framework.Networking.Serializers;

public class ClassicSerializer : ISerializer
{
    private readonly List<PacketSerializerInfo> _serializers = [];

    public ClassicSerializer()
    {
        AddPacketSerializer<TextPacket>(0, new TextPacketSerializer());
        AddPacketSerializer<TestDataPacket>(9, new TestDataPacketSerializer());
    }

    private void AddPacketSerializer<TPacket>(byte id, IPacketSerializer serializer) where TPacket : BasePacket
    {
        if (_serializers.Any(x => x.PacketId == id))
            throw new NetworkDataException($"A packet serializer for id {id} has already been registered");

        if (_serializers.Any(x => x.PacketType == typeof(TPacket)))
            throw new NetworkDataException($"A packet serializer for type {typeof(TPacket).Name} has already been registered");

        _serializers.Add(new PacketSerializerInfo(id, typeof(TPacket), serializer));
    }

    private PacketSerializerInfo FindPacketSerializer(byte id)
    {
        return _serializers.FirstOrDefault(x => x.PacketId == id)
            ?? throw new NetworkDataException($"There is no registered packet serializer for packet id {id}");
    }

    private PacketSerializerInfo FindPacketSerializer(Type type)
    {
        return _serializers.FirstOrDefault(x => x.PacketType == type)
            ?? throw new NetworkDataException($"There is no registered packet serializer for packet type {type}");
    }

    public byte[] Serialize(BasePacket packet)
    {
        PacketSerializerInfo info = FindPacketSerializer(packet.GetType());

        byte[] data = info.Serializer.Serialize(packet);
        byte[] length = BitConverter.GetBytes((ushort)data.Length);

        return [.. length, info.PacketId, .. data ];

    }

    public IEnumerable<BasePacket> Deserialize(byte[] data)
    {
        int startIdx = 0;

        while (startIdx < data.Length - 3)
        {
            ushort length = BitConverter.ToUInt16(data, startIdx);
            byte type = data[startIdx + 2];
            byte[] bytes = data[(startIdx + 3)..(startIdx + 3 + length)];
            startIdx += 3 + length;

            PacketSerializerInfo info = FindPacketSerializer(type);
            yield return info.Serializer.Deserialize(bytes);
        }
    }





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

    public class TestDataPacketSerializer : IPacketSerializer
    {
        public byte[] Serialize(BasePacket packet)
        {
            TestDataPacket p = (TestDataPacket)packet;

            byte[] name = Encoding.UTF8.GetBytes(p.Name);
            byte[] points = BitConverter.GetBytes(p.Points);
            byte[] time = BitConverter.GetBytes(p.TimeStamp.Ticks);

            return [(byte)name.Length, .. name, .. points, .. time];
        }

        public BasePacket Deserialize(byte[] data)
        {
            byte length = data[0];
            string name = Encoding.UTF8.GetString(data, 1, length);
            int points = BitConverter.ToInt32(data, 1 + length);
            long ticks = BitConverter.ToInt64(data, 5 + length);

            return new TestDataPacket()
            {
                Name = name,
                Points = points,
                TimeStamp = new DateTime(ticks)
            };
        }
    }

    class PacketSerializerInfo(byte id, Type type, IPacketSerializer serializer)
    {
        public byte PacketId { get; } = id;

        public Type PacketType { get; } = type;

        public IPacketSerializer Serializer { get; } = serializer;
    }
}
