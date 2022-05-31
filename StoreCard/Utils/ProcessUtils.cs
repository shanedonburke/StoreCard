#region

using System;
using System.Diagnostics;
using System.Linq;

#endregion

namespace StoreCard.Utils;

/// <summary>
/// Utilities for working with system processes.
/// </summary>
public static class ProcessUtils
{
    /// <summary>
    /// Kill all other StoreCard processes, if any are running.
    /// </summary>
    public static void KillOtherStoreCardProcesses()
    {
        Process[] otherProcesses = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);

        foreach (Process? process in otherProcesses)
        {
            if (process.Id != Environment.ProcessId)
            {
                process.Kill();
            }
        }
    }

    /// <summary>
    /// Check if one or more processes with the given name are running.
    /// </summary>
    /// <param name="name">Process name</param>
    /// <returns><c>true</c> if any process with that name is running</returns>
    public static bool IsProcessWithNameRunning(string name) => Process.GetProcessesByName(name).Any();
}
