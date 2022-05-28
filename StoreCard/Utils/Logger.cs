using System;
using System.Diagnostics;

namespace StoreCard.Utils;

public sealed class Logger
{
    public static void Log(string message)
    {
        Debug.WriteLine($"{DateTime.Now}: {message}");
    }

    public static void LogExceptionMessage(string explanation, Exception e)
    {
        Log($"{explanation} ({e.Message})");
    }
}
