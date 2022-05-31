#region

using System;
using System.Diagnostics;
using System.Linq;

#endregion

namespace StoreCard.Utils;

public static class ProcessUtils
{
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

    public static bool IsProcessWithNameRunning(string name) => Process.GetProcessesByName(name).Any();
}
