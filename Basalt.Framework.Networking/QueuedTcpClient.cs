using System.Collections.Generic;
using System.Net.Sockets;

namespace Basalt.Framework.Networking;

public class QueuedTcpClient : TcpClient
{
    private readonly List<byte> _queue;

    public QueuedTcpClient(string hostname, int port) : base(hostname, port)
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
