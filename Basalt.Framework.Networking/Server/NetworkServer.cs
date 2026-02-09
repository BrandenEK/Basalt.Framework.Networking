using Basalt.Framework.Networking.Exceptions;
using Basalt.Framework.Networking.Serializers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Basalt.Framework.Networking.Server;

public class NetworkServer
{
    private readonly IMessageSerializer _serializer;

    private TcpListener _listener;
    private Dictionary<string, QueuedTcpClient> _clients;

    public string Ip { get; private set; } = string.Empty;
    public int Port { get; private set; } = -1;
    public bool IsActive { get; private set; } = false;

    public NetworkServer(IMessageSerializer serializer)
    {
        _serializer = serializer;
    }

    public void Start(int port)
    {
        if (IsActive)
            throw new ListenerStatusException("Can't start if the server is already active");

        _clients = [];

        _listener = new TcpListener(IPAddress.Any, port);
        _listener.Server.NoDelay = true;
        _listener.Start();

        string address = _listener.LocalEndpoint.ToString();
        Ip = address.Substring(0, address.IndexOf(':'));
        Port = int.Parse(address.Substring(address.IndexOf(':') + 1));
        IsActive = true;

        OnServerStarted?.Invoke(address);
    }

    public void Stop()
    {
        if (!IsActive)
            throw new ListenerStatusException("Can't stop if the server is already inactive");

        foreach (var client in _clients.Values)
            client.Close();
        _clients.Clear();
        _clients = null;

        _listener.Stop();
        _listener = null;

        string address = $"{Ip}:{Port}";
        Ip = string.Empty;
        Port = -1;
        IsActive = false;

        OnServerStopped?.Invoke(address);
    }

    public bool Send(string ip, BasePacket packet)
    {
        if (!IsActive)
            return false;

        byte[] data = _serializer.Serialize(packet);

        if (_clients.TryGetValue(ip, out QueuedTcpClient client))
            client.Enqueue(data);

        return true;
    }

    public bool Send(IEnumerable<string> ips, BasePacket packet)
    {
        if (!IsActive)
            return false;

        byte[] data = _serializer.Serialize(packet);

        foreach (string ip in ips)
        {
            if (_clients.TryGetValue(ip, out QueuedTcpClient client))
                client.Enqueue(data);
        }

        return true;
    }

    public bool Broadcast(BasePacket packet)
    {
        if (!IsActive)
            return false;

        byte[] data = _serializer.Serialize(packet);

        foreach (var client in _clients.Values)
            client.Enqueue(data);

        return true;
    }

    public bool Update()
    {
        if (!IsActive)
            return false;

        foreach (var client in _clients.Values)
            client.Update();

        return true;
    }

    public bool Receive()
    {
        if (!IsActive)
            return false;

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

        return true;
    }

    private void ConnectClient(TcpClient client)
    {
        string ip = client.Client.RemoteEndPoint.ToString();
        _clients.Add(ip, new QueuedTcpClient(client));
        OnClientConnected?.Invoke(ip);
    }

    private void DisconnectClient(string ip)
    {
        _clients.Remove(ip);
        OnClientDisconnected?.Invoke(ip);
    }

    public delegate void ServerDelegate(string ip);
    public event ServerDelegate OnServerStarted;
    public event ServerDelegate OnServerStopped;

    public delegate void ClientDelegate(string ip);
    public event ClientDelegate OnClientConnected;
    public event ClientDelegate OnClientDisconnected;

    public delegate void ReceiveDelegate(string ip, BasePacket packet);
    public event ReceiveDelegate OnPacketReceived;
}
