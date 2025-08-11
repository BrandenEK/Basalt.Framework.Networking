using System.Threading;

namespace Basalt.Framework.Networking.Client;

public class NetworkClientWithThread : NetworkClient
{
    private readonly Thread _thread;
    private readonly int _readInterval;

    public NetworkClientWithThread(string ip, int port, int readInterval) : base(ip, port)
    {
        _thread = StartReadThread();
        _readInterval = readInterval;
    }

    private Thread StartReadThread()
    {
        Thread thread = new Thread(ReadLoop);
        thread.IsBackground = true;
        thread.Start();

        return thread;
    }

    private void ReadLoop()
    {
        while (IsActive)
        {
            Update();
            Receive();
            Thread.Sleep(_readInterval);
        }
    }
}
