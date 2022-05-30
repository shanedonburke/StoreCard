#region

using System.Collections.Generic;
using Microsoft.Win32;
using StoreCard.Models.Items.Installed.Games;
using StoreCard.Utils;

#endregion

namespace StoreCard.GameLibraries.Ea;

/// <summary>
/// Represents the library for the EA Desktop and Origin game launchers. The logic
/// for both launchers is the same.
/// </summary>
public sealed class EaLibrary : GameLibrary
{
    public override IEnumerable<InstalledGame> GetInstalledGames()
    {
        if (EaLaunchers.GetLauncher() == null)
        {
            Logger.Log("No EA launchers are installed.");
            yield break;
        }

        // Both launchers use the same key for installed games
        using RegistryKey? gameListKey = Registry.LocalMachine.OpenSubKey(
            RegUtils.BuildRegistryPath(RegUtils.Paths.Software32, "Origin Games"));

        if (gameListKey == null)
        {
            Logger.Log("Failed to get Origin games from the registry.");
            yield break;
        }

        // The sub-keys are app IDs with a display name property. We need both
        foreach (string appId in gameListKey.GetSubKeyNames())
        {
            using RegistryKey? gameKey = gameListKey.OpenSubKey(appId);

            if (gameKey?.GetValue("DisplayName") is not string gameName)
            {
                Logger.Log("Failed to get display name for Origin game.");
                continue;
            }

            yield return new InstalledEaGame(gameName, null, appId);
        }
    }
}
