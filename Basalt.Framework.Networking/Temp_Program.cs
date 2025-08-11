using Basalt.Framework.Networking.Client;
using Basalt.Framework.Networking.Serializers;
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

        var client = new NetworkClientWithThread("localhost", 33000, 1000);
        client.OnPacketReceived += Client_OnDataReceived;
        client.OnDisconnected += Client_OnDisconnected;

        Temp_Logger.Info($"Connected client to {client.Ip}:{client.Port}");

        while (true)
        {
            string input = Console.ReadLine()!;
            client.Send(new TextPacket()
            {
                Text = input
            });
        }
    }

    private static void Client_OnDisconnected()
    {
        // This probably never happens because it throws an error first
        Temp_Logger.Info("Disconnected from server");
    }

    private static void Client_OnDataReceived(BasePacket packet)
    {
        if (packet is not TextPacket tpacket)
            throw new Exception("only text");

        Temp_Logger.Warn($"[SERVER] {tpacket.Text}");
    }

    static void StartServer()
    {
        Console.Title = "Networking server";

        var server = new NetworkServerWithThread(33000, 1000);
        server.OnPacketReceived += Server_OnDataReceived;
        server.OnClientConnected += Server_OnClientConnected;
        server.OnClientDisconnected += Server_OnClientDisconnected;

        Temp_Logger.Info($"Started server at {server.Ip}:{server.Port}");

        while (true)
        {
            string input = Console.ReadLine()!;
            server.Broadcast(new TextPacket()
            {
                Text = input
            });
        }
    }

    private static void Server_OnClientDisconnected(string ip)
    {
        Temp_Logger.Info($"Client disconnected: {ip}");
    }

    private static void Server_OnClientConnected(string ip)
    {
        Temp_Logger.Info($"Client connected: {ip}");
    }

    private static void Server_OnDataReceived(string ip, BasePacket packet)
    {
        if (packet is not TextPacket tpacket)
            throw new Exception("only text");

        Temp_Logger.Warn($"[{ip}] {tpacket.Text}");
    }
}
