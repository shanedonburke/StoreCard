using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using StoreCard.Models.Items.Installed;
using StoreCard.Utils;

namespace StoreCard.GameLibraries.Epic;

internal class EpicLibrary : GameLibrary
{
    public override IEnumerable<InstalledGame> GetInstalledGames()
    {
        string programDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

        string launcherInstalledPath = Path.Combine(programDataFolder,
            @"Epic\UnrealEngineLauncher\LauncherInstalled.dat");
        string manifestFolderPath = Path.Combine(programDataFolder, @"Epic\EpicGamesLauncher\Data\Manifests");

        if (!File.Exists(launcherInstalledPath) || !Directory.Exists(manifestFolderPath))
        {
            yield break;
        }

        if (JsonConvert.DeserializeObject<EpicLauncherInstalled>(
                File.ReadAllText(launcherInstalledPath)) is not { } launcherInstalled)
        {
            yield break;
        }

        var appNames = launcherInstalled.InstallationList.Select(app => app.AppName).ToList();

        IEnumerable<string> manifestPaths = Directory.EnumerateFiles(
            manifestFolderPath,
            "*.item",
            SearchOption.TopDirectoryOnly);

        foreach (string manifestPath in manifestPaths)
        {
            if (JsonConvert.DeserializeObject<EpicManifest>(File.ReadAllText(manifestPath)) is not { } manifest)
            {
                yield break;
            }

            if (!appNames.Contains(manifest.AppName)) continue;

            string execPath = Path.Combine(manifest.InstallLocation, manifest.LaunchExecutable);
            if (!File.Exists(execPath)) continue;

            var hIcon = System.Drawing.Icon.ExtractAssociatedIcon(execPath);

            BitmapSource icon = Imaging.CreateBitmapSourceFromHIcon(
                hIcon!.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            icon.Freeze();

            yield return new InstalledEpicGame(manifest.DisplayName, icon, manifest.AppName);
        }
    }
}
