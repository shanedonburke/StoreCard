using System;
using System.Diagnostics;
using System.Linq;

namespace StoreCard.Utils;

internal class ProcessUtils
{
    public static void KillOtherStoreCardProcesses() {
        var otherProcesses = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
        foreach (var process in otherProcesses) {
            if (process.Id != Environment.ProcessId) {
                process.Kill();
            }
        }
    }

    public static bool IsProcessWithNameRunning(string name)
    {
        return Process.GetProcessesByName(name).Any();
    }
}
