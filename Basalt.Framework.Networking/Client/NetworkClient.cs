using Basalt.Framework.Networking.Extensions;
using Basalt.Framework.Networking.Serializers;
using System.Net.Sockets;

namespace Basalt.Framework.Networking.Client;

public class NetworkClient
{
    private readonly ISerializer _serializer = new SimpleTextSerializer();

    private readonly TcpClient _client;
    public bool IsActive { get; private set; }

    public string Ip { get; }
    public int Port { get; }

    public NetworkClient(string ip, int port)
    {
        _client = new TcpClient(ip, port);
        _client.NoDelay = true;
        _client.Client.NoDelay = true;

        Ip = ip;
        Port = port;

        IsActive = true;
    }

    public void Disconnect()
    {
        _client.Close();

        IsActive = false;
    }

    public void Send(BasePacket packet)
    {
        CheckConnectionStatus();

        if (!IsActive)
            throw new NetworkSendException();

        byte[] data = _serializer.Serialize(packet);
        _client.GetStream().Write(data, 0, data.Length);
    }

    public void Receive()
    {
        CheckConnectionStatus();

        if (!IsActive)
            throw new NetworkReceiveException();

        if (_client.Available == 0)
            return;

        byte[] buffer = new byte[_client.Available];
        _client.Client.Receive(buffer, 0, buffer.Length, SocketFlags.None);

        BasePacket packet = _serializer.Deserialize(buffer);
        OnPacketReceived?.Invoke(packet);
    }

    private void CheckConnectionStatus()
    {
        if (IsActive && !_client.Client.IsConnected())
        {
            Disconnect();
        }
    }

    public delegate void ReceiveDelegate(BasePacket packet);
    public event ReceiveDelegate? OnPacketReceived;
}
