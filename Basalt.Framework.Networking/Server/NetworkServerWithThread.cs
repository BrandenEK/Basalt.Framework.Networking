using System.Threading;

namespace Basalt.Framework.Networking.Server;

public class NetworkServerWithThread : NetworkServer
{
    private readonly Thread _thread;
    private readonly int _readInterval;

    public NetworkServerWithThread(int port, int readInterval) : base(port)
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
