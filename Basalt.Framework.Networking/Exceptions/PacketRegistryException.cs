
namespace Basalt.Framework.Networking.Exceptions;

public class PacketRegistryException : NetworkException
{
    public PacketRegistryException(string type) : base($"A packet serializer for {type} has already been registered") { }
}
