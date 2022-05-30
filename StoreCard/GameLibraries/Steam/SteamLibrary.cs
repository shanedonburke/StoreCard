#region

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using StoreCard.Models.Items.Installed.Games;
using StoreCard.Utils;

#endregion

namespace StoreCard.GameLibraries.Steam;

/// <summary>
/// Represents the library for the Steam game launcher.
/// </summary>
public sealed class SteamLibrary : GameLibrary
{
    /// <summary>
    /// The folder where Steam is installed, or null if it isn't installed.
    /// </summary>
    public static readonly string? SteamInstallFolder =
        // The folder path may be found in one of two places in the registry
        Registry.GetValue(
            RegUtils.BuildRegistryPath(
                RegUtils.Keys.HkeyLocalMachine,
                RegUtils.Paths.Software32,
                "Valve",
                "Steam"
            ),
            "InstallPath",
            null) as string ??
        Registry.GetValue(
            RegUtils.BuildRegistryPath(
                RegUtils.Keys.HkeyLocalMachine,
                RegUtils.Paths.Software64,
                "Valve",
                "Steam"
            ),
            "InstallPath",
            null) as string;

    public override IEnumerable<InstalledGame> GetInstalledGames()
    {
        if (SteamInstallFolder == null)
        {
            yield break;
        }

        // Folder where game icons are stored
        string libraryCacheFolder = $"{SteamInstallFolder}\\appcache\\librarycache";

        // Folder where steam app data is stored
        string steamAppsFolder = $"{SteamInstallFolder}\\steamapps";

        // Manifest file with info about all installed games
        string libraryFoldersPath = $"{steamAppsFolder}\\libraryfolders.vdf";

        if (!File.Exists(libraryFoldersPath))
        {
            Logger.Log("Steam is installed, but the game manifest couldn't be found.");
            yield break;
        }

        var libraryFolders = SteamDictionary.Parse(File.ReadAllText(libraryFoldersPath));

        // Paths to folders where Steam games are installed
        var steamAppsFolderPaths = libraryFolders.Children
            // Integer keys (e.g., "0") represent folders where games are installed, e.g., on different drives
            .Where(c => int.TryParse(c.Key, out _))
            // The "path" value omits the "steamapps" folder, which is always present
            .Select(c => $"{c.Value.Pairs["path"]}\\steamapps")
            .ToList();

        // Each .acf file is a manifest for a single game
        IEnumerable<string> gameManifestPaths = steamAppsFolderPaths.SelectMany(appsFolderPath =>
            Directory.EnumerateFiles(appsFolderPath, "*.acf", SearchOption.TopDirectoryOnly));

        foreach (string? manifestPath in gameManifestPaths)
        {
            if (!File.Exists(manifestPath))
            {
                continue;
            }

            var gameManifest = SteamDictionary.Parse(File.ReadAllText(manifestPath));

            string name = gameManifest.Pairs["name"];
            string appId = gameManifest.Pairs["appid"];

            // Find the game icon in the cache folder
            string iconPath = $"{libraryCacheFolder}\\{appId}_icon.jpg";

            if (!File.Exists(iconPath))
            {
                yield return new InstalledSteamGame(name, null, appId);
                continue;
            }

            Stream imageStreamSource = new FileStream(
                iconPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);
            var decoder = new JpegBitmapDecoder(
                imageStreamSource,
                BitmapCreateOptions.PreservePixelFormat,
                BitmapCacheOption.Default);
            BitmapSource bitmapIcon = decoder.Frames[0];

            // From https://stackoverflow.com/a/61681670
            var cached = new CachedBitmap(
                bitmapIcon,
                BitmapCreateOptions.None,
                BitmapCacheOption.OnLoad);
            cached.Freeze();

            yield return new InstalledSteamGame(name, cached, appId);
        }
    }
}
