using System;
using System.Diagnostics;
using System.Linq;

namespace StoreCard.Utils;

public class ProcessUtils
{
    public static void KillOtherStoreCardProcesses() {
        Process[] otherProcesses = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
        foreach (Process? process in otherProcesses) {
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
