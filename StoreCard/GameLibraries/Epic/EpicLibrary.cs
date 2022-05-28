using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using StoreCard.Models.Items.Installed.Games;
using StoreCard.Static;
using StoreCard.Utils;

namespace StoreCard.GameLibraries.Epic;

internal class EpicLibrary : GameLibrary
{
    private static readonly string s_launcherInstalledPath = Path.Combine(
        FolderPaths.CommonApplicationData,
        @"Epic\UnrealEngineLauncher\LauncherInstalled.dat");

    private static readonly string s_manifestFolderPath = Path.Combine(
        FolderPaths.CommonApplicationData,
        @"Epic\EpicGamesLauncher\Data\Manifests");

    public override IEnumerable<InstalledGame> GetInstalledGames()
    {
        if (!File.Exists(s_launcherInstalledPath) || !Directory.Exists(s_manifestFolderPath))
        {
            yield break;
        }

        EpicLauncherInstalled? launcherInstalled;

        try
        {
            launcherInstalled = JsonConvert.DeserializeObject<EpicLauncherInstalled>(
                File.ReadAllText(s_launcherInstalledPath));
        }
        catch (JsonSerializationException e)
        {
            Debug.WriteLine("Failed to deserialize Epic Games data file:");
            Debug.WriteLine(e.Message);
            yield break;
        }

        if (launcherInstalled == null)
        {
            yield break;
        }

        var appNames = launcherInstalled.InstallationList.Select(app => app.AppName).ToList();

        IEnumerable<string> manifestPaths = Directory.EnumerateFiles(
            s_manifestFolderPath,
            "*.item",
            SearchOption.TopDirectoryOnly);

        foreach (string manifestPath in manifestPaths)
        {
            if (!File.Exists(manifestPath))
            {
                continue;
            }

            EpicManifest? manifest;

            try
            {
                manifest = JsonConvert.DeserializeObject<EpicManifest>(File.ReadAllText(manifestPath));
            }
            catch (JsonSerializationException e)
            {
                Debug.WriteLine("Failed to deserialize Epic Games manifest:");
                Debug.WriteLine(e.Message);
                yield break;
            }

            if (manifest == null)
            {
                continue;
            }

            if (!appNames.Contains(manifest.AppName)) continue;

            string execPath = Path.Combine(manifest.InstallLocation, manifest.LaunchExecutable);
            if (!File.Exists(execPath)) continue;

            BitmapSource icon = IconUtils.GetFileIconByPath(execPath)!;

            yield return new InstalledEpicGame(manifest.DisplayName, icon, manifest.AppName);
        }
    }
}
