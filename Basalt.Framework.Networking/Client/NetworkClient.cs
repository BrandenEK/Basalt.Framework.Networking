using Basalt.Framework.Networking.Serializers;
using System.Net.Sockets;

namespace Basalt.Framework.Networking.Client;

public class NetworkClient
{
    private readonly IMessageSerializer _serializer = new ClassicSerializer();

    private QueuedTcpClient _client;

    public string Ip { get; private set; }
    public int Port { get; private set; }
    public bool IsActive { get; private set; }

    //public NetworkClient(string ip, int port)
    //{
    //    _client = new QueuedTcpClient(new TcpClient(ip, port));

    //    Ip = ip;
    //    Port = port;

    //    IsActive = true;
    //}

    public void Connect(string ip, int port)
    {
        _client = new QueuedTcpClient(new TcpClient(ip, port));

        Ip = ip;
        Port = port;
        IsActive = true;

        OnConnected?.Invoke();
    }

    public void Disconnect()
    {
        _client.Close();
        _client = null;

        Ip = string.Empty;
        Port = -1;
        IsActive = false;

        OnDisconnected?.Invoke();
    }

    public void Send(BasePacket packet)
    {
        byte[] data = _serializer.Serialize(packet);
        _client.Enqueue(data);
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

    public delegate void ConnectDelegate();
    public event ConnectDelegate OnConnected;
    public event ConnectDelegate OnDisconnected;

    public delegate void ReceiveDelegate(BasePacket packet);
    public event ReceiveDelegate OnPacketReceived;
}
