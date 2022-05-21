// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using StoreCard.Models.Items.Installed;

namespace StoreCard.GameLibraries;

internal class EaLibrary : GameLibrary
{
    public static readonly string? EaLauncherPath =
        Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Electronic Arts\EA Desktop", "LauncherAppPath",
            null) as string;

    public override IEnumerable<InstalledGame> GetInstalledGames()
    {
        if (EaLauncherPath == null) yield break;

        using RegistryKey? gameListKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Origin Games");
        if (gameListKey == null) yield break;

        foreach (string gameId in gameListKey.GetSubKeyNames())
        {
            using RegistryKey? gameKey = gameListKey.OpenSubKey(gameId);

            if (gameKey?.GetValue("DisplayName") is not string gameName) continue;

            yield return new InstalledEaGame(gameName, gameId, null);
        }
    }
}
