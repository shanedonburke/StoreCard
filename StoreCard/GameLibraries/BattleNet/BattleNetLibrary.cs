// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using StoreCard.Models.Items.Installed;
using StoreCard.Static;
using StoreCard.Utils;

namespace StoreCard.GameLibraries.BattleNet;

internal class BattleNetLibrary : GameLibrary
{
    public static readonly string? BattleNetInstallFolder = Registry.GetValue(
        RegUtils.BuildRegistryPath(
            RegUtils.Keys.HkeyLocalMachine,
            RegUtils.Paths.Uninstall32,
            "Battle.net"),
        "InstallLocation",
        null) as string;

    public override IEnumerable<InstalledGame> GetInstalledGames()
    {
        if (BattleNetInstallFolder == null) yield break;

        using RegistryKey? uninstallKey = Registry.LocalMachine.OpenSubKey(RegUtils.Paths.Uninstall32);
        if (uninstallKey == null) yield break;

        foreach (string programKeyName in uninstallKey.GetSubKeyNames())
        {
            using RegistryKey? programKey = uninstallKey.OpenSubKey(programKeyName);

            if (programKey?.GetValue("Publisher") is not "Blizzard Entertainment") continue;

            if (programKey.GetValue("UninstallString") is not string uninstallString) continue;

            Match match = Regex.Match(uninstallString, @"--uid=(.*?)\s");
            if (!match.Success) continue;

            string uid = match.Groups[1].Value;

            if (BattleNetGameIds.GetGameId(uid) is not { } gameId) continue;

            if (programKey.GetValue("DisplayName") is not string displayName) continue;

            if (programKey.GetValue("DisplayIcon") is not string displayIconPath) continue;

            if (IconUtils.GetFileIconByPath(displayIconPath) is not { } icon) continue;

            yield return new InstalledBattleNetGame(displayName, icon, gameId);
        }
    }
}
