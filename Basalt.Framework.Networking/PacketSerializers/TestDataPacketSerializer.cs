using Basalt.Framework.Networking.Packets;
using Basalt.Framework.Networking.Streams;
using System;

namespace Basalt.Framework.Networking.PacketSerializers;

public class TestDataPacketSerializer : IPacketSerializer
{
    public byte[] Serialize(BasePacket packet)
    {
        TestDataPacket p = (TestDataPacket)packet;

        var stream = new OutStream();
        stream.Write_string(p.Name);
        stream.Write_int(p.Points);
        stream.Write_long(p.TimeStamp.Ticks);

        return stream;
    }

    public BasePacket Deserialize(byte[] data)
    {
        var stream = new InStream(data);

        string name = stream.Read_string();
        int points = stream.Read_int();
        long ticks = stream.Read_long();

        return new TestDataPacket()
        {
            Name = name,
            Points = points,
            TimeStamp = new DateTime(ticks)
        };
    }
}
