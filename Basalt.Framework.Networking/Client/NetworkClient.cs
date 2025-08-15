using Basalt.Framework.Networking.Serializers;
using System.Net.Sockets;

namespace Basalt.Framework.Networking.Client;

public class NetworkClient
{
    private readonly IMessageSerializer _serializer;
    private QueuedTcpClient _client;

    public string Ip { get; private set; }
    public int Port { get; private set; }
    public bool IsActive { get; private set; }

    public NetworkClient(IMessageSerializer serializer)
    {
        _serializer = serializer;
    }

    public void Connect(string ip, int port)
    {
        if (IsActive)
            throw new NetworkException("Can't connect if the client is already active");

        _client = new QueuedTcpClient(new TcpClient(ip, port));

        string server = $"{ip}:{port}";
        Ip = ip;
        Port = port;
        IsActive = true;

        OnConnected?.Invoke(server);
    }

    public void Disconnect()
    {
        if (!IsActive)
            throw new NetworkException("Can't disconnect if the client is already inactive");

        _client.Close();
        _client = null;

        string server = $"{Ip}:{Port}";
        Ip = string.Empty;
        Port = -1;
        IsActive = false;

        OnDisconnected?.Invoke(server);
    }

    public bool Send(BasePacket packet)
    {
        // Don't check connection status

        if (!IsActive)
            return false;

        byte[] data = _serializer.Serialize(packet);
        _client.Enqueue(data);
        return true;
    }

    public bool Update()
    {
        CheckConnectionStatus();

        if (!IsActive)
            return false;

        _client.Update();
        return true;
    }

    public bool Receive()
    {
        CheckConnectionStatus();

        if (!IsActive)
            return false;

        if (!_client.TryReceive(out byte[] data))
            return true;

        foreach (var packet in _serializer.Deserialize(data))
            OnPacketReceived?.Invoke(packet);
        return true;
    }

    private void CheckConnectionStatus()
    {
        if (IsActive && !_client.IsConnected)
        {
            Disconnect();
        }
    }

    public delegate void ConnectDelegate(string server);
    public event ConnectDelegate OnConnected;
    public event ConnectDelegate OnDisconnected;

    public delegate void ReceiveDelegate(BasePacket packet);
    public event ReceiveDelegate OnPacketReceived;
}
