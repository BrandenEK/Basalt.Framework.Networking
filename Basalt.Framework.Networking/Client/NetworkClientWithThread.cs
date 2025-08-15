using Basalt.Framework.Networking.Serializers;
using System.Threading;

namespace Basalt.Framework.Networking.Client;

public class NetworkClientWithThread : NetworkClient
{
    private readonly int _readInterval;

    public NetworkClientWithThread(int readInterval, IMessageSerializer s) : base(s)
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
                try
                {
                    Receive();
                }
                catch (NetworkException ex)
                {
                    Temp_Logger.Error($"Error during deserialization: {ex.Message}");
                }
                catch (System.Exception ex)
                {
                    Temp_Logger.Info($"Error during deserialization: {ex}");
                }

                Update();
            }
            
            Thread.Sleep(_readInterval);
        }
    }
}
