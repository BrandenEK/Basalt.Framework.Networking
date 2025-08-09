using Basalt.Framework.Networking.Extensions;
using System.Net.Sockets;

namespace Basalt.Framework.Networking.Client;

public class NetworkClient
{
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

    public void Send(byte[] data)
    {
        if (!IsActive)
            throw new NetworkSendException();

        CheckConnectionStatus();

        _client.GetStream().Write(data, 0, data.Length);
    }

    public void Receive()
    {
        if (!IsActive)
            throw new NetworkReceiveException();

        CheckConnectionStatus();

        if (_client.Available == 0)
            return;

        byte[] buffer = new byte[_client.Available];
        _client.Client.Receive(buffer, 0, buffer.Length, SocketFlags.None);

        OnDataReceived?.Invoke(buffer);
    }

    private void CheckConnectionStatus()
    {
        if (IsActive && !_client.Client.IsConnected())
        {
            Disconnect();
            throw new NetworkDisconnectException();
        }
    }

    public delegate void ReceiveDelegate(byte[] data);
    public event ReceiveDelegate? OnDataReceived;
}
