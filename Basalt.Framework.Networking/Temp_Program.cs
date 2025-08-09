using Basalt.Framework.Networking.Client;
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

        var client = new NetworkClient("localhost", 33000);
        client.OnDataReceived += Client_OnDataReceived;

        Temp_Logger.Info($"Connected client to {client.Ip}:{client.Port}");

        while (true)
        {
            Console.ReadLine();
        }
    }

    private static void Client_OnDataReceived(byte[] data)
    {
        throw new NotImplementedException();
    }

    static void StartServer()
    {
        Console.Title = "Networking server";

        var server = new NetworkServerWithThread(33000);
        server.OnDataReceived += Server_OnDataReceived;

        Temp_Logger.Info($"Started server at {server.Ip}:{server.Port}");

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
