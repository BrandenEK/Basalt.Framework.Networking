
namespace Basalt.Framework.Networking.Exceptions;

internal class UnknownPacketTypeException : NetworkException
{
    public UnknownPacketTypeException(string type) : base($"There is no registered packet serializer for packet {type}") { }
}
