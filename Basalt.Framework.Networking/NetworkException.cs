using System;

namespace Basalt.Framework.Networking;

public class NetworkException(string message) : Exception(message) { }
