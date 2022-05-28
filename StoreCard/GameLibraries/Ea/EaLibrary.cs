using System.Collections.Generic;
using Microsoft.Win32;
using StoreCard.Models.Items.Installed.Games;
using StoreCard.Utils;

namespace StoreCard.GameLibraries.Ea;

public sealed class EaLibrary : GameLibrary
{
    public override IEnumerable<InstalledGame> GetInstalledGames()
    {
        if (EaLaunchers.GetLauncher() == null)
        {
            Logger.Log("No EA launchers are installed.");
            yield break;
        }

        using RegistryKey? gameListKey = Registry.LocalMachine.OpenSubKey(
            RegUtils.BuildRegistryPath(RegUtils.Paths.Software32, "Origin Games"));

        if (gameListKey == null)
        {
            Logger.Log("Failed to get Origin games from the registry.");
            yield break;
        }

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
