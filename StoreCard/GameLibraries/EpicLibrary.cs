using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using StoreCard.Models.Games.Epic;
using StoreCard.Models.Items.Installed;

namespace StoreCard.GameLibraries;

internal class EpicLibrary : GameLibrary
{
    public override IEnumerable<InstalledGame> GetInstalledGames()
    {
        var installedGames = new List<InstalledGame>();

        var programDataFolder = Environment.ExpandEnvironmentVariables("%ProgramData%");

        var launcherInstalledPath = Path.Combine(programDataFolder,
            @"Epic\UnrealEngineLauncher\LauncherInstalled.dat");
        var manifestFolderPath = Path.Combine(programDataFolder, @"Epic\EpicGamesLauncher\Data\Manifests");

        if (!File.Exists(launcherInstalledPath) || !Directory.Exists(manifestFolderPath)) {
            return installedGames;
        }

        if (JsonConvert.DeserializeObject<EpicLauncherInstalled>(File.ReadAllText(launcherInstalledPath)) is not { } launcherInstalled) {
            return installedGames;
        }

        var appNames = launcherInstalled.InstallationList.Select(app => app.AppName).ToList();

        var manifestPaths = Directory.GetFiles(manifestFolderPath, "*.item");

        foreach (var manifestPath in manifestPaths) {
            if (JsonConvert.DeserializeObject<EpicManifest>(File.ReadAllText(manifestPath)) is not { } manifest) {
                return installedGames;
            }

            if (!appNames.Contains(manifest.AppName)) continue;

            var execPath = Path.Combine(manifest.InstallLocation, manifest.LaunchExecutable);
            if (!File.Exists(execPath)) continue;

            var hIcon = System.Drawing.Icon.ExtractAssociatedIcon(execPath);
            var icon = Imaging.CreateBitmapSourceFromHIcon(
                hIcon!.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            icon.Freeze();

            installedGames.Add(new InstalledEpicGame(manifest.DisplayName, icon, manifest.AppName));
        }

        return installedGames;
    }
}