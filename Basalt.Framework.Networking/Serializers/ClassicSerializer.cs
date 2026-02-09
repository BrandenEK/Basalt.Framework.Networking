using Basalt.Framework.Networking.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Basalt.Framework.Networking.Serializers;

public class ClassicSerializer : IMessageSerializer
{
    private readonly List<PacketSerializerInfo> _serializers = [];

    public ClassicSerializer RegisterPacket<TPacket>(byte id, IPacketSerializer serializer) where TPacket : BasePacket
    {
        if (_serializers.Any(x => x.PacketId == id))
            throw new PacketRegistryException($"id={id}");

        if (_serializers.Any(x => x.PacketType == typeof(TPacket)))
            throw new PacketRegistryException($"type={typeof(TPacket).Name}");

        _serializers.Add(new PacketSerializerInfo(id, typeof(TPacket), serializer));
        return this;
    }

    public ClassicSerializer RegisterPacket<TPacket>(byte id, Func<BasePacket> creator) where TPacket : BasePacket
    {
        if (_serializers.Any(x => x.PacketId == id))
            throw new PacketRegistryException($"A packet serializer for id {id} has already been registered");

        if (_serializers.Any(x => x.PacketType == typeof(TPacket)))
            throw new PacketRegistryException($"A packet serializer for type {typeof(TPacket).Name} has already been registered");

        _serializers.Add(new PacketSerializerInfo(id, typeof(TPacket), new ReflectionPacketSerializer(creator)));
        return this;
    }

    private PacketSerializerInfo FindPacketSerializer(byte id)
    {
        return _serializers.FirstOrDefault(x => x.PacketId == id)
            ?? throw new UnknownPacketTypeException($"id={id}");
    }

    private PacketSerializerInfo FindPacketSerializer(Type type)
    {
        return _serializers.FirstOrDefault(x => x.PacketType == type)
            ?? throw new UnknownPacketTypeException($"type={type.Name}");
    }

    public byte[] Serialize(BasePacket packet)
    {
        PacketSerializerInfo info = FindPacketSerializer(packet.GetType());

        byte[] data = info.Serializer.Serialize(packet);
        byte[] length = BitConverter.GetBytes((ushort)data.Length);

        return [.. length, info.PacketId, .. data];
    }

    public IEnumerable<BasePacket> Deserialize(byte[] data)
    {
        int startIdx = 0;

        while (startIdx < data.Length - 3)
        {
            ushort length = BitConverter.ToUInt16(data, startIdx);

            if (startIdx + 3 + length > data.Length)
                throw new DeserializationException("Packet length will overflow buffer", data);

            byte type = data[startIdx + 2];
            byte[] bytes = new byte[length];
            for (int i = 0; i < length; i++)
                bytes[i] = data[startIdx + 3 + i];

            startIdx += 3 + length;

            PacketSerializerInfo info = FindPacketSerializer(type);
            yield return info.Serializer.Deserialize(bytes);
        }

        if (startIdx != data.Length)
            throw new DeserializationException("There was extraneous data in the buffer", data);
    }

    class PacketSerializerInfo(byte id, Type type, IPacketSerializer serializer)
    {
        public byte PacketId { get; } = id;

        public Type PacketType { get; } = type;

        public IPacketSerializer Serializer { get; } = serializer;
    }
}
