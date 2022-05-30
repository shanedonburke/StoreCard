#region

using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using StoreCard.Models.Items.Installed.Games;
using StoreCard.Utils;

#endregion

namespace StoreCard.GameLibraries.Itch;

/// <summary>
/// Represents the library for the itch (i.e., itch.io) game launcher.
/// </summary>
public sealed class ItchLibrary : GameLibrary
{
    private static bool IsInstalled => ButlerPaths.ButlerExecutable != null && File.Exists(ButlerPaths.ButlerDatabase);

    public override IEnumerable<InstalledGame> GetInstalledGames()
    {
        if (!IsInstalled)
        {
            yield break;
        }

        ButlerClient client = new();
        client.Start();

        // Must authenticate before sending any other request
        if (!client.Authenticate())
        {
            Logger.Log("Failed to authenticate with Butler client.");
            yield break;
        }

        // "Caves" are installed games/programsa
        if (client.FetchCaves() is not { } caves)
        {
            Logger.Log("Failed to fetch caves.");
            yield break;
        }

        foreach (ButlerCave cave in caves)
        {
            string installFolder = cave.InstallInfo.InstallFolder;

            BitmapSource? icon = null;

            // We don't have a good way to find the actual executable (we don't need it to launch the game), so we'll
            // use the first exe we find with a non-default icon. It may or may not be the right one... ¯\_(ツ)_/¯
            foreach (string exePath in Directory.EnumerateFiles(installFolder, "*.exe", SearchOption.AllDirectories))
            {
                var hIcon = Icon.ExtractAssociatedIcon(exePath);

                if (hIcon == SystemIcons.Application)
                {
                    continue;
                }

                icon = IconUtils.CreateBitmapSourceFromHIcon(hIcon ?? SystemIcons.Application);
            }

            yield return new InstalledItchGame(cave.Game.Title, icon, cave.Id);
        }

        client.KillDaemon();
    }
}
