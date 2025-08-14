using Basalt.Framework.Networking.Serializers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Basalt.Framework.Networking.Server;

public class NetworkServer
{
    private readonly ISerializer _serializer = new ClassicSerializer();

    private readonly TcpListener _listener;
    private readonly Dictionary<string, QueuedTcpClient> _clients = [];
    public bool IsActive { get; private set; }

    public string Ip { get; }
    public int Port { get; }

    public NetworkServer(int port)
    {
        _listener = new TcpListener(IPAddress.Any, port);
        _listener.Server.NoDelay = true;
        _listener.Start();

        string[] parts = _listener.LocalEndpoint.ToString()!.Split(':');
        Ip = parts[0];
        Port = int.Parse(parts[1]);

        IsActive = true;
    }

    public void Stop()
    {
        IsActive = false;

        foreach (var client in _clients.Values)
            client.Close();
        _clients.Clear();
        _listener.Stop();
    }

    public void Send(string ip, BasePacket packet)
    {
        byte[] data = _serializer.Serialize(packet);

        if (_clients.TryGetValue(ip, out QueuedTcpClient? client))
            client.Enqueue(data);
    }

    public void Broadcast(BasePacket packet)
    {
        byte[] data = _serializer.Serialize(packet);

        foreach (var client in _clients.Values)
            client.Enqueue(data);
    }

    public void Update()
    {
        if (!IsActive)
            throw new NetworkSendException();

        foreach (var client in _clients.Values)
            client.Update();
    }

    public void Receive()
    {
        if (!IsActive)
            throw new NetworkReceiveException();

        // Check for new connections
        if (_listener.Pending())
        {
            ConnectClient(_listener.AcceptTcpClient());
        }

        // Remove all clients that have been disconnected
        foreach (string ip in _clients.Where(kvp => !kvp.Value.IsConnected).Select(kvp => kvp.Key))
        {
            DisconnectClient(ip);
        }

        // Read data from all client streams
        foreach (var kvp in _clients)
        {
            if (!kvp.Value.TryReceive(out byte[] data))
                continue;

            foreach (var packet in _serializer.Deserialize(data))
                OnPacketReceived?.Invoke(kvp.Key, packet);
        }
    }

    private void ConnectClient(TcpClient client)
    {
        string ip = client.Client.RemoteEndPoint!.ToString()!;
        _clients.Add(ip, new QueuedTcpClient(client));
        OnClientConnected?.Invoke(ip);
    }

    private void DisconnectClient(string ip)
    {
        _clients.Remove(ip);
        OnClientDisconnected?.Invoke(ip);
    }

    public delegate void ConnectDelegate(string ip);
    public event ConnectDelegate? OnClientConnected;
    public event ConnectDelegate? OnClientDisconnected;

    public delegate void ReceiveDelegate(string ip, BasePacket packet);
    public event ReceiveDelegate? OnPacketReceived;
}
