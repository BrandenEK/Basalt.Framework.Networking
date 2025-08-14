using System.Threading;

namespace Basalt.Framework.Networking.Client;

public class NetworkClientWithThread : NetworkClient
{
    private readonly int _readInterval;

    public NetworkClientWithThread(string ip, int port, int readInterval) : base(ip, port)
    {
        _readInterval = readInterval;
        StartReadThread();
    }

    private void StartReadThread()
    {
        var thread = new Thread(ReadLoop);
        thread.IsBackground = true;
        thread.Start();
    }

    private void ReadLoop()
    {
        while (IsActive)
        {
            Receive();
            Update();
            Thread.Sleep(_readInterval);
        }
    }
}
