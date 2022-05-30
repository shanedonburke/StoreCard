#region

using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using StoreCard.Models.Items.Installed.Games;
using StoreCard.Utils;

#endregion

namespace StoreCard.GameLibraries.BattleNet;

/// <summary>
/// Represents the library for the Battle.net game launcher.
/// </summary>
public sealed class BattleNetLibrary : GameLibrary
{
    /// <summary>
    /// The folder in which the launcher is installed.
    /// </summary>
    public static readonly string? BattleNetInstallFolder = Registry.GetValue(
        RegUtils.BuildRegistryPath(
            RegUtils.Keys.HkeyLocalMachine,
            RegUtils.Paths.Uninstall32,
            "Battle.net"),
        "InstallLocation",
        null) as string;

    /// <summary>
    /// Finds the UID of a game from a registry value that includes the UID as an argument.
    /// </summary>
    private static readonly Regex s_uidRegex = new(@"--uid=(?<uid>.*?)\s");

    private static bool IsInstalled => BattleNetInstallFolder != null;

    public override IEnumerable<InstalledGame> GetInstalledGames()
    {
        if (!IsInstalled)
        {
            Logger.Log("The Battle.net launcher is not installed.");
            yield break;
        }

        // A key that includes info on most installed programs (not just Battle.net games)
        using RegistryKey? uninstallKey = Registry.LocalMachine.OpenSubKey(
            RegUtils.Paths.Uninstall32);

        if (uninstallKey == null)
        {
            Logger.Log("The list of programs could not be obtained from the registry.");
            yield break;
        }

        foreach (string programKeyName in uninstallKey.GetSubKeyNames())
        {
            // The key for a given program
            using RegistryKey? programKey = uninstallKey.OpenSubKey(programKeyName);

            // All Battle.net games have this as the "Publisher" value. This is a pretty
            // reliable way to find them
            if (programKey?.GetValue("Publisher") is not "Blizzard Entertainment")
            {
                continue;
            }

            // All programs should have this key
            if (programKey.GetValue("DisplayName") is not string displayName)
            {
                Logger.Log("Failed to get the display name of a Battle.net game.");
                continue;
            }

            // A command that invokes the Battle.net uninstaller to uninstall the game.
            // This can be used to obtain the game UID
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
