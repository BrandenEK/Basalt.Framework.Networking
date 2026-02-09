using System;

namespace Basalt.Framework.Networking;

public abstract class NetworkException(string message) : Exception(message) { }
