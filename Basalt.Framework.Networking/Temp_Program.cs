using Basalt.Framework.Networking.Server;
using System;

namespace Basalt.Framework.Networking;

internal class Temp_Program
{
    static void Main(string[] args)
    {
#if RELEASE
        StartServer();
#else
        StartClient();
#endif
    }

    static void StartClient()
    {
        Console.Title = "Networking client";
    }

    static void StartServer()
    {
        Console.Title = "Networking server";

        var server = new NetworkServer(33000);
        server.OnDataReceived += Server_OnDataReceived;

        Temp_Logger.Info($"Started server at {server.Ip}");

        while (true)
        {
            Console.ReadLine();
        }
    }

    private static void Server_OnDataReceived(string ip, byte[] data)
    {
        Temp_Logger.Warn($"Received {data.Length} bytes from {ip}");
    }
}
