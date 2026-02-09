using System.Linq;

namespace Basalt.Framework.Networking.Exceptions;

internal class DeserializationException : NetworkException
{
    public DeserializationException(string message, byte[] data) : base($"{message} [{string.Join(" ", data.Select(x => x.ToString()).ToArray())}]") { }
}
