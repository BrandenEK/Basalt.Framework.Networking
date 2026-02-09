using Basalt.Framework.Networking.Streams;
using System;
using System.Linq;
using System.Reflection;

namespace Basalt.Framework.Networking.Serializers;

public class ReflectionPacketSerializer : IPacketSerializer
{
    private readonly Func<BasePacket> _creator;

    public ReflectionPacketSerializer(Func<BasePacket> creator)
    {
        _creator = creator;
    }

    public byte[] Serialize(BasePacket packet)
    {
        var properties = packet.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .OrderBy(p => p.Name);

        var stream = new OutStream();

        foreach (var p in properties)
        {
            TypeCode type = Type.GetTypeCode(p.PropertyType);

            switch (type)
            {
                case TypeCode.Byte:
                    stream.Write_byte((byte)p.GetValue(packet, null));
                    break;
                case TypeCode.SByte:
                    stream.Write_sbyte((sbyte)p.GetValue(packet, null));
                    break;
                case TypeCode.UInt16:
                    stream.Write_ushort((ushort)p.GetValue(packet, null));
                    break;
                case TypeCode.Int16:
                    stream.Write_short((short)p.GetValue(packet, null));
                    break;
                case TypeCode.UInt32:
                    stream.Write_uint((uint)p.GetValue(packet, null));
                    break;
                case TypeCode.Int32:
                    stream.Write_int((int)p.GetValue(packet, null));
                    break;
                case TypeCode.UInt64:
                    stream.Write_ulong((ulong)p.GetValue(packet, null));
                    break;
                case TypeCode.Int64:
                    stream.Write_long((long)p.GetValue(packet, null));
                    break;
                case TypeCode.Single:
                    stream.Write_float((float)p.GetValue(packet, null));
                    break;
                case TypeCode.Double:
                    stream.Write_double((double)p.GetValue(packet, null));
                    break;
                case TypeCode.Boolean:
                    stream.Write_bool((bool)p.GetValue(packet, null));
                    break;
                case TypeCode.Char:
                    stream.Write_char((char)p.GetValue(packet, null));
                    break;
                case TypeCode.String:
                    stream.Write_string((string)p.GetValue(packet, null));
                    break;
                default:
                    throw new Exception($"Can not serialize a property of type {type}");
            }
        }

        return stream;
    }

    public BasePacket Deserialize(byte[] data)
    {
        var packet = _creator();

        var properties = packet.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .OrderBy(p => p.Name);

        var stream = new InStream(data);

        foreach (var p in properties)
        {
            TypeCode type = Type.GetTypeCode(p.PropertyType);

            switch (type)
            {
                case TypeCode.Byte:
                    p.SetValue(packet, stream.Read_byte(), null);
                    break;
                case TypeCode.SByte:
                    p.SetValue(packet, stream.Read_sbyte(), null);
                    break;
                case TypeCode.UInt16:
                    p.SetValue(packet, stream.Read_ushort(), null);
                    break;
                case TypeCode.Int16:
                    p.SetValue(packet, stream.Read_short(), null);
                    break;
                case TypeCode.UInt32:
                    p.SetValue(packet, stream.Read_uint(), null);
                    break;
                case TypeCode.Int32:
                    p.SetValue(packet, stream.Read_int(), null);
                    break;
                case TypeCode.UInt64:
                    p.SetValue(packet, stream.Read_ulong(), null);
                    break;
                case TypeCode.Int64:
                    p.SetValue(packet, stream.Read_long(), null);
                    break;
                case TypeCode.Single:
                    p.SetValue(packet, stream.Read_float(), null);
                    break;
                case TypeCode.Double:
                    p.SetValue(packet, stream.Read_double(), null);
                    break;
                case TypeCode.Boolean:
                    p.SetValue(packet, stream.Read_bool(), null);
                    break;
                case TypeCode.Char:
                    p.SetValue(packet, stream.Read_char(), null);
                    break;
                case TypeCode.String:
                    p.SetValue(packet, stream.Read_string(), null);
                    break;
                default:
                    throw new Exception($"Can not deserialize a property of type {type}");
            }
        }

        return packet;
    }
}
