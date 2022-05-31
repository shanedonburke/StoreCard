#region

using System;
using System.Diagnostics;

#endregion

namespace StoreCard.Utils;

/// <summary>
/// A simple logger that prints messages with a timestamp.
/// </summary>
public static class Logger
{
    /// <summary>
    /// Print a debug message with a timestamp.
    /// </summary>
    /// <param name="message"></param>
    public static void Log(string message) => Debug.WriteLine($"{DateTime.Now}: {message}");

    /// <summary>
    /// Prints the given exception's message alongside an explanation of what caused it.
    /// </summary>
    /// <param name="explanation">Brief explanation of exception</param>
    /// <param name="e">The exception</param>
    public static void LogExceptionMessage(string explanation, Exception e) => Log($"{explanation} ({e.Message})");
}
