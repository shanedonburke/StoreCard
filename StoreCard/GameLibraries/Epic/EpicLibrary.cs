#region

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using StoreCard.Models.Items.Installed.Games;
using StoreCard.Static;
using StoreCard.Utils;

#endregion

namespace StoreCard.GameLibraries.Epic;

public sealed class EpicLibrary : GameLibrary
{
    private static readonly string s_launcherInstalledPath = Path.Combine(
        FolderPaths.CommonApplicationData,
        @"Epic\UnrealEngineLauncher\LauncherInstalled.dat");

    private static readonly string s_manifestFolderPath = Path.Combine(
        FolderPaths.CommonApplicationData,
        @"Epic\EpicGamesLauncher\Data\Manifests");

    private static bool IsInstalled => File.Exists(s_launcherInstalledPath) &&
                                       Directory.Exists(s_manifestFolderPath);

    public override IEnumerable<InstalledGame> GetInstalledGames()
    {
        if (!IsInstalled)
        {
            Logger.Log("The Epic Games launcher is not installed.");
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
            Logger.LogExceptionMessage("Failed to deserialize Epic Games data file", e);
            yield break;
        }

        if (launcherInstalled == null)
        {
            Logger.Log("Failed to deserialize Epic Games data file.");
            yield break;
        }

        var appNames = launcherInstalled.InstallationList
            .Select(app => app.AppName)
            .ToList();

        IEnumerable<string> manifestPaths = Directory.EnumerateFiles(
            s_manifestFolderPath,
            "*.item",
            SearchOption.TopDirectoryOnly);

        foreach (string manifestPath in manifestPaths)
        {
            if (!File.Exists(manifestPath))
            {
                Logger.Log($"The Epic Games manifest at {manifestPath} does not exist.");
                continue;
            }

            EpicManifest? manifest;

            try
            {
                manifest = JsonConvert.DeserializeObject<EpicManifest>(File.ReadAllText(manifestPath));
            }
            catch (JsonSerializationException e)
            {
                Logger.LogExceptionMessage($"Failed to deserialize Epic Games manifest at {manifestPath}", e);
                yield break;
            }

            if (manifest == null)
            {
                Logger.Log($"Failed to deserialize Epic Games manifest at {manifestPath}");
                continue;
            }

            string appName = manifest.AppName;

            if (!appNames.Contains(appName))
            {
                Logger.Log($"An unexpected Epic Games manifest was found for {appName}.");
                continue;
            }

            string execPath = Path.Combine(manifest.InstallLocation, manifest.LaunchExecutable);

            if (!File.Exists(execPath))
            {
                Logger.Log($"The executable for {appName} from Epic Games could not be found.");
                continue;
            }

            BitmapSource icon = IconUtils.GetFileIconByPath(execPath)!;

            yield return new InstalledEpicGame(manifest.DisplayName, icon, manifest.AppName);
        }
    }
}
