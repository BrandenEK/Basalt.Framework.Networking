using Basalt.Framework.Networking.Client;
using Basalt.Framework.Networking.Server;
using System;
using System.Text;

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

        var client = new NetworkClientWithThread("localhost", 33000, 1000);
        client.OnDataReceived += Client_OnDataReceived;

        Temp_Logger.Info($"Connected client to {client.Ip}:{client.Port}");

        while (true)
        {
            string input = Console.ReadLine()!;
            client.Send(Encoding.UTF8.GetBytes(input));
        }
    }

    private static void Client_OnDataReceived(byte[] data)
    {
        Temp_Logger.Warn($"[SERVER] {Encoding.UTF8.GetString(data)}");
    }

    static void StartServer()
    {
        Console.Title = "Networking server";

        var server = new NetworkServerWithThread(33000, 1000);
        server.OnDataReceived += Server_OnDataReceived;

        Temp_Logger.Info($"Started server at {server.Ip}:{server.Port}");

        while (true)
        {
            string input = Console.ReadLine()!;
            server.Broadcast(Encoding.UTF8.GetBytes(input));
        }
    }

    private static void Server_OnDataReceived(string ip, byte[] data)
    {
        Temp_Logger.Warn($"[{ip}] {Encoding.UTF8.GetString(data)}");
    }
}
