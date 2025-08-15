using System;

namespace Basalt.Framework.Networking;

public class NetworkException(string message) : Exception(message) { }

public class NetworkDataException : NetworkException
{
    public NetworkDataException(string message) : base(message) { }
}
