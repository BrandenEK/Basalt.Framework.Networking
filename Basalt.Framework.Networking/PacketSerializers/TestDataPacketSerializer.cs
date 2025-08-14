using Basalt.Framework.Networking.Packets;
using System;
using System.Text;

namespace Basalt.Framework.Networking.PacketSerializers;

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
