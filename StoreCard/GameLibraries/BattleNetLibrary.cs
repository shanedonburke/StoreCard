// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.Win32;
using StoreCard.Models.Items.Installed;
using StoreCard.Static;

namespace StoreCard.GameLibraries;

internal class BattleNetLibrary : GameLibrary
{
    public static readonly string? BattleNetInstallFolder =
        Registry.GetValue("HKEY_LOCAL_MACHINE" + RegistryKeys.SoftwareUninstall + @"\Battle.net", "InstallLocation", null) as string;

    public override IEnumerable<InstalledGame> GetInstalledGames()
    {
        if (BattleNetInstallFolder == null) yield break;

        using RegistryKey? uninstallKey = Registry.LocalMachine.OpenSubKey(RegistryKeys.SoftwareUninstall);
        if (uninstallKey == null) yield break;

        foreach (string programKeyName in uninstallKey.GetSubKeyNames())
        {
            using RegistryKey? programKey = uninstallKey.OpenSubKey(programKeyName);

            if (programKey?.GetValue("Publisher") is not string publisher) continue;

            if (publisher != "Blizzard Entertainment") continue;

            
        }
    }
}
