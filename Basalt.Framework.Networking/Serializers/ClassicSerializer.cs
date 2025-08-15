﻿using Basalt.Framework.Networking.PacketSerializers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Basalt.Framework.Networking.Serializers;

public class ClassicSerializer : IMessageSerializer
{
    private readonly List<PacketSerializerInfo> _serializers = [];

    public ClassicSerializer AddPacketSerializer<TPacket>(byte id, IPacketSerializer serializer) where TPacket : BasePacket
    {
        if (_serializers.Any(x => x.PacketId == id))
            throw new NetworkException($"A packet serializer for id {id} has already been registered");

        if (_serializers.Any(x => x.PacketType == typeof(TPacket)))
            throw new NetworkException($"A packet serializer for type {typeof(TPacket).Name} has already been registered");

        _serializers.Add(new PacketSerializerInfo(id, typeof(TPacket), serializer));
        return this;
    }

    private PacketSerializerInfo FindPacketSerializer(byte id)
    {
        return _serializers.FirstOrDefault(x => x.PacketId == id)
            ?? throw new NetworkException($"There is no registered packet serializer for packet id {id}");
    }

    private PacketSerializerInfo FindPacketSerializer(Type type)
    {
        return _serializers.FirstOrDefault(x => x.PacketType == type)
            ?? throw new NetworkException($"There is no registered packet serializer for packet type {type}");
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
            byte type = data[startIdx + 2];
            byte[] bytes = data[(startIdx += 3)..(startIdx += length)];

            PacketSerializerInfo info = FindPacketSerializer(type);
            yield return info.Serializer.Deserialize(bytes);
        }
    }

    class PacketSerializerInfo(byte id, Type type, IPacketSerializer serializer)
    {
        public byte PacketId { get; } = id;

        public Type PacketType { get; } = type;

        public IPacketSerializer Serializer { get; } = serializer;
    }
}
