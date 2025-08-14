using System;
using System.Text;

namespace Basalt.Framework.Networking.Streams;

/// <summary>
/// A readable data stream
/// </summary>
public class InStream
{
    private readonly byte[] _bytes;
    private int _pointer = 0;

    private InStream(byte[] bytes)
    {
        _bytes = bytes;
    }

    private void ValidateReadLength(int size)
    {
        if (_pointer + size > _bytes.Length)
            throw new NetworkDataException("Can not read past the length of the stream");
    }

    private T Read_generic<T>(int size, Func<byte[], int, T> func)
    {
        ValidateReadLength(size);

        T value = func(_bytes, _pointer);
        _pointer += size;
        return value;
    }

    /// <summary>
    /// Reads the next byte in the stream
    /// </summary>
    public byte Read_byte()
    {
        ValidateReadLength(1);
        return _bytes[_pointer++];
    }

    /// <summary>
    /// Reads the next sbyte in the stream
    /// </summary>
    public sbyte Read_sbyte()
    {
        ValidateReadLength(1);
        return (sbyte)_bytes[_pointer++];
    }

    /// <summary>
    /// Reads the next ushort in the stream
    /// </summary>
    public ushort Read_ushort()
    {
        return Read_generic(2, BitConverter.ToUInt16);
    }

    /// <summary>
    /// Reads the next short in the stream
    /// </summary>
    public short Read_short()
    {
        return Read_generic(2, BitConverter.ToInt16);
    }

    /// <summary>
    /// Reads the next uint in the stream
    /// </summary>
    public uint Read_uint()
    {
        return Read_generic(4, BitConverter.ToUInt32);
    }

    /// <summary>
    /// Reads the next int in the stream
    /// </summary>
    public int Read_int()
    {
        return Read_generic(4, BitConverter.ToInt32);
    }

    /// <summary>
    /// Reads the next ulong in the stream
    /// </summary>
    public ulong Read_ulong()
    {
        return Read_generic(8, BitConverter.ToUInt64);
    }

    /// <summary>
    /// Reads the next long in the stream
    /// </summary>
    public long Read_long()
    {
        return Read_generic(8, BitConverter.ToInt64);
    }

    /// <summary>
    /// Reads the next float in the stream
    /// </summary>
    public float Read_float()
    {
        return Read_generic(4, BitConverter.ToSingle);
    }

    /// <summary>
    /// Reads the next double in the stream
    /// </summary>
    public double Read_double()
    {
        return Read_generic(8, BitConverter.ToDouble);
    }

    /// <summary>
    /// Reads the next char in the stream
    /// </summary>
    public char Read_char()
    {
        ValidateReadLength(1);
        return Convert.ToChar(_bytes[_pointer++]);
    }

    /// <summary>
    /// Reads the next string in the stream
    /// </summary>
    public string Read_string()
    {
        byte length = Read_byte();

        if (length == 0)
            return string.Empty;

        ValidateReadLength(length);

        string value = Encoding.UTF8.GetString(_bytes, _pointer, length);
        _pointer += length;
        return value;
    }

    /// <summary>
    /// Reads the next data in the stream
    /// </summary>
    public byte[] Read_data(int length)
    {
        ValidateReadLength(length);

        byte[] value = _bytes[_pointer..(_pointer + length)];
        _pointer += length;
        return value;
    }

    /// <summary>
    /// Creates a new InStream from a byte[]
    /// </summary>
    public static implicit operator InStream(byte[] bytes) => new InStream(bytes);
}
