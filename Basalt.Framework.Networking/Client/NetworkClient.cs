using Basalt.Framework.Networking.Serializers;
using System.Net.Sockets;

namespace Basalt.Framework.Networking.Client;

public class NetworkClient
{
    private readonly ISerializer _serializer = new SimpleTextSerializer();

    private readonly QueuedTcpClient _client;
    public bool IsActive { get; private set; }

    public string Ip { get; }
    public int Port { get; }

    public NetworkClient(string ip, int port)
    {
        _client = new QueuedTcpClient(new TcpClient(ip, port));

        Ip = ip;
        Port = port;

        IsActive = true;
    }

    public void Disconnect()
    {
        IsActive = false;

        _client.Close();
        OnDisconnected?.Invoke();
    }

    public void Send(BasePacket packet)
    {
        byte[] data = _serializer.Serialize(packet);
        _client.Enqueue(data);
    }

    public void Update()
    {
        CheckConnectionStatus();

        if (!IsActive)
            throw new NetworkSendException();

        _client.Update();
    }

    public void Receive()
    {
        CheckConnectionStatus();

        if (!IsActive)
            throw new NetworkReceiveException();

        if (!_client.TryReceive(out byte[] data))
            return;

        foreach (var packet in _serializer.Deserialize(data))
            OnPacketReceived?.Invoke(packet);
    }

    private void CheckConnectionStatus()
    {
        if (IsActive && !_client.IsConnected)
        {
            Disconnect();
        }
    }

    public delegate void ConnectDelegate();
    public event ConnectDelegate? OnDisconnected;

    public delegate void ReceiveDelegate(BasePacket packet);
    public event ReceiveDelegate? OnPacketReceived;
}
