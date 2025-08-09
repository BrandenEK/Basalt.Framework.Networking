using System.Threading;

namespace Basalt.Framework.Networking.Server;

public class NetworkServerWithThread
{
    private readonly NetworkServer _server;
    private readonly Thread _thread;
    private readonly int _readInterval;

    public NetworkServerWithThread(int port, int readInterval)
    {
        _server = new NetworkServer(port);
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
        while (_server.IsActive)
        {
            _server.Receive();
            Thread.Sleep(_readInterval);
        }
    }
}
