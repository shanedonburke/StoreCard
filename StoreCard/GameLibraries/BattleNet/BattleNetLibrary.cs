using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using StoreCard.Models.Items.Installed.Games;
using StoreCard.Utils;

namespace StoreCard.GameLibraries.BattleNet;

public sealed class BattleNetLibrary : GameLibrary
{
    public static readonly string? BattleNetInstallFolder = Registry.GetValue(
        RegUtils.BuildRegistryPath(
            RegUtils.Keys.HkeyLocalMachine,
            RegUtils.Paths.Uninstall32,
            "Battle.net"),
        "InstallLocation",
        null) as string;

    private static readonly Regex s_uidRegex = new(@"--uid=(?<uid>.*?)\s");

    private static bool IsInstalled => BattleNetInstallFolder != null;

    public override IEnumerable<InstalledGame> GetInstalledGames()
    {
        if (!IsInstalled)
        {
            Logger.Log("The Battle.net launcher is not installed.");
            yield break;
        }

        using RegistryKey? uninstallKey = Registry.LocalMachine.OpenSubKey(
            RegUtils.Paths.Uninstall32);

        if (uninstallKey == null)
        {
            Logger.Log("The list of programs could not be obtained from the registry.");
            yield break;
        }

        foreach (string programKeyName in uninstallKey.GetSubKeyNames())
        {
            using RegistryKey? programKey = uninstallKey.OpenSubKey(programKeyName);

            if (programKey?.GetValue("Publisher") is not "Blizzard Entertainment")
            {
                continue;
            }

            if (programKey.GetValue("DisplayName") is not string displayName)
            {
                Logger.Log("Failed to get the display name of a Battle.net game.");
                continue;
            }

            if (programKey.GetValue("UninstallString") is not string uninstallString)
            {
                Logger.Log($"Failed to get uninstall string for {displayName}.");
                continue;
            }

            Match match = s_uidRegex.Match(uninstallString);

            if (!match.Success)
            {
                Logger.Log($"Failed to get UID for {displayName}.");
                continue;
            }

            string uid = match.Groups["uid"].Value;

            if (BattleNetGameIds.GetGameId(uid) is not { } gameId)
            {
                Logger.Log($"Failed to get game ID for {displayName} (from UID {uid}).");
                continue;
            }

            if (programKey.GetValue("DisplayIcon") is not string displayIconPath)
            {
                Logger.Log($"Failed to get display icon for {displayName}");
                continue;
            }

            if (IconUtils.GetFileIconByPath(displayIconPath) is not { } icon)
            {
                Logger.Log($"Failed to get file icon for {displayIconPath}.");
                continue;
            }

            yield return new InstalledBattleNetGame(displayName, icon, gameId);
        }
    }
}
