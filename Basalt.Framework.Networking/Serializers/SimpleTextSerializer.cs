using System.Text;

namespace Basalt.Framework.Networking.Serializers;

public class SimpleTextSerializer : ISerializer
{
    public BasePacket Deserialize(byte[] data)
    {
        string text = Encoding.UTF8.GetString(data);
        return new TextPacket()
        {
            Text = text
        };
    }

    public byte[] Serialize(BasePacket packet)
    {
        if (packet is not TextPacket tpacket)
            throw new System.Exception("Only text packets");

        return Encoding.UTF8.GetBytes(tpacket.Text ?? string.Empty);
    }
}

public class TextPacket : BasePacket
{
    public string Text { get; set; } = string.Empty;
}
