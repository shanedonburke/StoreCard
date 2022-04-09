using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreCard
{
    internal class Paths
    {
        public static readonly string? SteamInstallFolder;

        static Paths()
        {
            // Try 64-bit key, then 32-bit
            SteamInstallFolder = (Registry.GetValue(
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam",
                "InstallPath",
                null) as string)
                ?? Registry.GetValue(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam",
                    "InstallPath",
                    null) as string;
        }
    }
}
