
namespace Basalt.Framework.Networking.Exceptions;

public class SerializationException : NetworkException
{
    public SerializationException(string message, object data) : base($"{message} [{data}]") { }
}
