using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using SteamKit2;
using StoreCard.Models.Items.Installed;
using StoreCard.Models.Items.Installed.Games;
using StoreCard.Utils;

namespace StoreCard.GameLibraries.Steam;

internal class SteamLibrary : GameLibrary
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

    private static bool IsFileReady(string filename)
    {
        // If the file can be opened for exclusive access it means that the file
        // is no longer locked by another process.
        try
        {
            using FileStream inputStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None);
            return inputStream.Length > 0;
        }
        catch
        {
            return false;
        }
    }

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

        var libraryFolders = KeyValue.LoadFromString(File.ReadAllText(libraryFoldersPath));

        if (libraryFolders == null)
        {
            yield break;
        }

        var steamAppsFolderPaths = libraryFolders.Children
            .Where(child => int.TryParse(child.Name, out _))
            .Select(kv => $"{kv["path"].Value}\\steamapps")
            .ToList();

        IEnumerable<string> manifestPaths = steamAppsFolderPaths.SelectMany(appsFolderPath =>
            Directory.EnumerateFiles(appsFolderPath, "*.acf", SearchOption.TopDirectoryOnly));

        foreach (string? manifestPath in manifestPaths)
        {
            if (!File.Exists(manifestPath))
            {
                continue;
            }

            var manifest = KeyValue.LoadFromString(File.ReadAllText(manifestPath));
            if (manifest == null) continue;

            string? name = manifest["name"].Value;
            string? appId = manifest["appid"].Value;
            if (name == null || appId == null) continue;

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
