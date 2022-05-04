using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreCard.Utils;

internal class Processes
{
    public static void KillOtherStoreCardProcesses() {
        var otherProcesses = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
        foreach (var process in otherProcesses) {
            if (process.Id != Environment.ProcessId) {
                process.Kill();
            }
        }
    }
}