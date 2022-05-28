using System;
using System.Diagnostics;
using System.Windows;
using Newtonsoft.Json;
using StoreCard.GameLibraries.Ea;
using StoreCard.Models.Items.Installed.Games;
using StoreCard.Services;
using StoreCard.Static;
using StoreCard.Utils;

namespace StoreCard.Models.Items.Saved.Games;

public sealed class SavedEaGame : SavedGame
{
    public readonly string AppId;

    [JsonConstructor]
    public SavedEaGame(string id, string name, long lastOpened, string appId) : base(id, name, null, lastOpened)
    {
        AppId = appId;
    }

    public SavedEaGame(InstalledEaGame game) : this(
        Guid.NewGuid().ToString(),
        game.Name,
        TimeUtils.UnixTimeMillis,
        game.AppId)
    {
    }

    public override SpecificItemCategory SpecificCategory => SpecificItemCategory.EaGame;

    public override string SecondaryText => GamePlatformNames.Ea;

    protected override void OpenProtected()
    {
        EaLaunchers.Launcher? launcher = EaLaunchers.GetLauncher();

        if (launcher == null)
        {
            MessageBoxService.Instance.ShowMessageBox(
                "No EA launcher could be found. Please install Origin or EA Desktop, then try again.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            return;
        }

        if (!launcher.IsRunning)
        {
            MessageBoxService.Instance.ShowMessageBox(
                "An EA launcher must be running to start this game. Please open Origin or EA Desktop, then try again.",
                "Warning",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        // If `LauncherPath` is null, `GetLauncher()` will have already returned null
        Process.Start(launcher.LauncherPath!, $"origin://LaunchGame/{AppId}");
    }
}
