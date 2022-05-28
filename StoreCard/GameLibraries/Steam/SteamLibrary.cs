using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using StoreCard.Models.Items.Installed.Games;
using StoreCard.Utils;

namespace StoreCard.GameLibraries.Steam;

internal sealed class SteamLibrary : GameLibrary
{
    public static readonly string? SteamInstallFolder =
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

        string libraryCacheFolder = $"{SteamInstallFolder}\\appcache\\librarycache";
        string steamAppsFolder = $"{SteamInstallFolder}\\steamapps";
        string libraryFoldersPath = $"{steamAppsFolder}\\libraryfolders.vdf";

        if (!File.Exists(libraryFoldersPath))
        {
            yield break;
        }

        var libraryFolders = SteamDictionary.Parse(File.ReadAllText(libraryFoldersPath));

        var steamAppsFolderPaths = libraryFolders.Children
            .Where(c => int.TryParse(c.Key, out _))
            .Select(c => $"{c.Value.Pairs["path"]}\\steamapps")
            .ToList();

        IEnumerable<string> manifestPaths = steamAppsFolderPaths.SelectMany(appsFolderPath =>
            Directory.EnumerateFiles(appsFolderPath, "*.acf", SearchOption.TopDirectoryOnly));

        foreach (string? manifestPath in manifestPaths)
        {
            if (!File.Exists(manifestPath))
            {
                continue;
            }

            var manifest = SteamDictionary.Parse(File.ReadAllText(manifestPath));

            string? name = manifest.Pairs["name"];
            string? appId = manifest.Pairs["appid"];

            string iconPath = $"{libraryCacheFolder}\\{appId}_icon.jpg";

            if (!File.Exists(iconPath))
            {
                continue;
            }

            Stream imageStreamSource = new FileStream(iconPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);
            var decoder = new JpegBitmapDecoder(imageStreamSource,
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
