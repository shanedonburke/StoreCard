using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using SteamKit2;
using StoreCard.Models.Items.Installed;

namespace StoreCard.GameLibraries;

internal class SteamLibrary : GameLibrary
{
    public static readonly string? SteamInstallFolder =
        Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam", "InstallPath", null) as string ??
        Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam", "InstallPath", null) as string;

    public override IEnumerable<InstalledGame> GetInstalledGames()
    {
        var installedGames = new List<InstalledGame>();

        if (SteamInstallFolder == null) yield break;

        var libraryCacheFolder = $"{SteamInstallFolder}\\appcache\\librarycache";
        var steamAppsFolder = $"{SteamInstallFolder}\\steamapps";

        var libraryFolders = KeyValue.LoadFromString(File.ReadAllText($"{steamAppsFolder}\\libraryfolders.vdf"));
        if (libraryFolders == null) yield break;

        var steamAppsFolderPaths = libraryFolders.Children
            .Where(child => int.TryParse(child.Name, out _))
            .Select(kv => $"{kv["path"].Value}\\steamapps")
            .ToList();

        foreach (var appsFolderPath in steamAppsFolderPaths)
        {
            var appManifestPaths = Directory.GetFiles(appsFolderPath, "*.acf", SearchOption.AllDirectories);
            foreach (var manifestPath in appManifestPaths)
            {
                var manifest = KeyValue.LoadFromString(File.ReadAllText(manifestPath));
                if (manifest == null) continue;

                var name = manifest["name"].Value;
                var appId = manifest["appid"].Value;
                if (name == null || appId == null) continue;

                var iconPath = $"{libraryCacheFolder}\\{appId}_icon.jpg";
                if (!File.Exists(iconPath)) continue;

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
}