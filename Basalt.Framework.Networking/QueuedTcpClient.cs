using Basalt.Framework.Networking.Extensions;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Basalt.Framework.Networking;

public class QueuedTcpClientx : TcpClient
{
    private readonly List<byte> _queue;

    public QueuedTcpClientx(string hostname, int port) : base(hostname, port)
    {
        NoDelay = true;
        Client.NoDelay = true;
        _queue = [];
    }

    public void Enqueue(byte[] data)
    {
        _queue.AddRange(data);
    }

    public void Clear()
    {
        _queue.Clear();
    }

    public void Update()
    {
        if (_queue.Count == 0)
            return;

        byte[] data = _queue.ToArray();
        GetStream().Write(data, 0, data.Length);
        _queue.Clear();
    }
}

public class QueuedTcpClient
{
    private readonly TcpClient _client;
    private readonly List<byte> _queue;

    public bool IsConnected => _client.Client.IsConnected();

    public QueuedTcpClient(TcpClient client)
    {
        _client = client;
        _client.NoDelay = true;
        _client.Client.NoDelay = true;
        _queue = [];
    }

    public void Enqueue(byte[] data)
    {
        _queue.AddRange(data);
    }

    public void Clear()
    {
        _queue.Clear();
    }

    public void Update()
    {
        if (_queue.Count == 0)
            return;

        byte[] data = _queue.ToArray();
        _client.GetStream().Write(data, 0, data.Length);
        _queue.Clear();
    }

    public bool TryReceive(out byte[] data)
    {
        if (_client.Available == 0)
        {
            data = null!;
            return false;
        }

        data = new byte[_client.Available];
        _client.Client.Receive(data, 0, data.Length, SocketFlags.None);
        return true;
    }

    public void Close()
    {
        _client.Close();
    }
}
