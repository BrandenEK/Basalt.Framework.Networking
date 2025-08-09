using System;

namespace Basalt.Framework.Networking;

public static class Temp_Logger
{
    public static void Info(object message)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(message);
    }

    public static void Warn(object message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(message);
    }

    public static void Error(object message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
    }
}
