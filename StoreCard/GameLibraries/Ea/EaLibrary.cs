// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using Microsoft.Win32;
using StoreCard.Models.Items.Installed;
using StoreCard.Utils;

namespace StoreCard.GameLibraries.Ea;

internal class EaLibrary : GameLibrary
{
    public override IEnumerable<InstalledGame> GetInstalledGames()
    {
        if (EaLaunchers.GetLauncher() == null) yield break;

        using RegistryKey? gameListKey = Registry.LocalMachine.OpenSubKey(
            RegUtils.BuildRegistryPath(RegUtils.Paths.Software32, "Origin Games"));

        if (gameListKey == null) yield break;

        foreach (string appId in gameListKey.GetSubKeyNames())
        {
            using RegistryKey? gameKey = gameListKey.OpenSubKey(appId);

            if (gameKey?.GetValue("DisplayName") is not string gameName) continue;

            yield return new InstalledEaGame(gameName, appId, null);
        }
    }
}
