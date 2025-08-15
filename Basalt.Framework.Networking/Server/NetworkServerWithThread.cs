using Basalt.Framework.Networking.Serializers;
using System.Threading;

namespace Basalt.Framework.Networking.Server;

public class NetworkServerWithThread : NetworkServer
{
    private readonly int _readInterval;

    public NetworkServerWithThread(IMessageSerializer s, int readInterval) : base(s)
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
        while (true)
        {
            if (IsActive)
            {
                Receive();
                Update();
            }
            
            Thread.Sleep(_readInterval);
        }
    }
}
