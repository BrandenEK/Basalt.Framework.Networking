using System;

namespace Basalt.Framework.Networking.Packets;

public class TestDataPacket : BasePacket
{
    public string Name { get; set; } = string.Empty;

    public int Points { get; set; } = 0;

    public DateTime TimeStamp { get; set; }
}
