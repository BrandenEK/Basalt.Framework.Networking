using System.Threading;

namespace Basalt.Framework.Networking.Server;

public class NetworkServerWithThread : NetworkServer
{
    private readonly int _readInterval;

    public NetworkServerWithThread(int port, int readInterval) : base(port)
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
