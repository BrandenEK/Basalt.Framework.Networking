using System;

namespace Basalt.Framework.Networking;

public static class Temp_Logger
{
    public static void Info(object message)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Print(message);
    }

    public static void Warn(object message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Print(message);
    }

    public static void Error(object message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Print(message);
    }

    public static void Print(object message)
    {
        Console.WriteLine($"[{DateTime.UtcNow:HH:mm:ss}] {message}");
    }
}
