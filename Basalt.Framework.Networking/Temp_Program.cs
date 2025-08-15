using Basalt.Framework.Networking.Client;
using Basalt.Framework.Networking.Packets;
using Basalt.Framework.Networking.PacketSerializers;
using Basalt.Framework.Networking.Serializers;
using Basalt.Framework.Networking.Server;
using System;
using System.Diagnostics;

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

        var client = new NetworkClientWithThread(1000, new ClassicSerializer()
            .AddPacketSerializer<TextPacket>(0, new TextPacketSerializer())
            .AddPacketSerializer<TestDataPacket>(9, new TestDataPacketSerializer()));

        client.OnPacketReceived += Client_OnDataReceived;
        client.OnClientConnected += Client_OnConnected;
        client.OnClientDisconnected += Client_OnDisconnected;

        while (true)
        {
            string input = Console.ReadLine()!;

            if (input == "test")
            {
                client.Send(new TestDataPacket()
                {
                    Name = string.Empty,
                    Points = 33000,
                    TimeStamp = DateTime.Now,
                });
                continue;
            }
            if (input == "c")
            {
                client.Connect("localhost", 33000);
                continue;
            }
            if (input == "d")
            {
                client.Disconnect();
                continue;
            }

            client.Send(new TextPacket()
            {
                Text = input
            });
        }
    }

    private static void Client_OnConnected(string ip)
    {
        Temp_Logger.Info($"Connected to {ip}");
    }

    private static void Client_OnDisconnected(string ip)
    {
        Temp_Logger.Info($"Disconnected from {ip}");
    }

    private static void Client_OnDataReceived(BasePacket packet)
    {
        if (packet is TextPacket tpacket)
        {
            Temp_Logger.Warn($"[SERVER] {tpacket.Text}");
            return;
        }

        if (packet is TestDataPacket datapacket)
        {
            Temp_Logger.Info($"Data from {datapacket.Name}: {datapacket.TimeStamp.ToShortDateString()}");
            return;
        }
    }

    static void StartServer()
    {
        Console.Title = "Networking server";

        var server = new NetworkServerWithThread(new ClassicSerializer()
            .AddPacketSerializer<TextPacket>(0, new TextPacketSerializer())
            .AddPacketSerializer<TestDataPacket>(9, new TestDataPacketSerializer()), 1000);

        server.OnPacketReceived += Server_OnDataReceived;
        server.OnServerStarted += Server_OnServerStarted;
        server.OnServerStopped += Server_OnServerStopped;
        server.OnClientConnected += Server_OnClientConnected;
        server.OnClientDisconnected += Server_OnClientDisconnected;

        server.Start(33000);

        while (true)
        {
            string input = Console.ReadLine()!;

            if (input == "test")
            {
                server.Broadcast(new TestDataPacket()
                {
                    Name = Environment.MachineName,
                    Points = 33000,
                    TimeStamp = DateTime.Now,
                });
                continue;
            }

            if (input == "speed")
            {
                ClassicSerializer serializer1 = new ClassicSerializer();
                StreamSerializer serializer2 = new StreamSerializer();

                TestDataPacket packet = new TestDataPacket()
                {
                    Name = "Test",
                    Points = 12345,
                    TimeStamp = DateTime.Now,
                };
                byte[] bytes = [0x11, 0x00, 0x09, 0x04, 0x54, 0x65, 0x73, 0x74, 0x39, 0x30, 0x00, 0x00, 0x05, 0x18, 0xDF, 0x76, 0xC5, 0xDA, 0xDD, 0x08];

                Stopwatch watch = Stopwatch.StartNew();
                for (int i = 0; i < 100000; i++)
                {
                    serializer1.Serialize(packet);
                }

                watch.Stop();
                Temp_Logger.Info($"Serialization 1 time: {watch.ElapsedTicks} ticks");

                watch.Restart();
                for (int i = 0; i < 100000; i++)
                {
                    serializer2.Serialize(packet);
                }

                watch.Stop();
                Temp_Logger.Info($"Serialization 2 time: {watch.ElapsedTicks} ticks");

                watch.Restart();
                for (int i = 0; i < 100000; i++)
                {
                    serializer1.Deserialize(bytes);
                }

                watch.Stop();
                Temp_Logger.Info($"Deserialization 1 time: {watch.ElapsedTicks} ticks");

                watch.Restart();
                for (int i = 0; i < 100000; i++)
                {
                    serializer2.Deserialize(bytes);
                }

                watch.Stop();
                Temp_Logger.Info($"Deserialization 2 time: {watch.ElapsedTicks} ticks");
                continue;
            }


            server.Broadcast(new TextPacket()
            {
                Text = input
            });
        }
    }

    private static void Server_OnServerStopped(string ip)
    {
        Temp_Logger.Info($"Stopped server at {ip}");
    }

    private static void Server_OnServerStarted(string ip)
    {
        Temp_Logger.Info($"Started server at {ip}");
    }

    private static void Server_OnClientDisconnected(string ip)
    {
        Temp_Logger.Info($"Client disconnected at {ip}");
    }

    private static void Server_OnClientConnected(string ip)
    {
        Temp_Logger.Info($"Client connected at {ip}");
    }

    private static void Server_OnDataReceived(string ip, BasePacket packet)
    {
        if (packet is TextPacket tpacket)
        {
            Temp_Logger.Warn($"[{ip}] {tpacket.Text}");
            return;
        }
        
        if (packet is TestDataPacket datapacket)
        {
            Temp_Logger.Info($"Data from {datapacket.Name}: {datapacket.TimeStamp.ToShortDateString()}");
            return;
        }
    }
}
