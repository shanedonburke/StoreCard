#region

using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using StoreCard.GameLibraries.Steam;
using StoreCard.Models.Items.Installed.Games;
using StoreCard.Services;
using StoreCard.Static;
using StoreCard.Utils;

#endregion

namespace StoreCard.Models.Items.Saved.Games;

public sealed class SavedSteamGame : SavedGame
{
    [JsonConstructor]
    public SavedSteamGame(
        string id,
        string name,
        string? base64Icon,
        string appId,
        long lastOpened) : base(id, name, base64Icon, lastOpened) => AppId = appId;

    public SavedSteamGame(InstalledSteamGame game) : base(
        Guid.NewGuid().ToString(),
        game.Name,
        game.BitmapIcon?.ToBase64(),
        TimeUtils.UnixTimeMillis
    ) =>
        AppId = game.AppId;

    public string AppId { get; }

    public override SpecificItemCategory SpecificCategory => SpecificItemCategory.SteamGame;

    public override string SecondaryText => GamePlatformNames.Steam;

    protected override void OpenProtected()
    {
        if (SteamLibrary.SteamInstallFolder == null)
        {
            MessageBoxService.Instance.ShowMessageBox(
                "The Steam installation folder could not be found.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            return;
        }

        string steamExecPath = Path.Combine(SteamLibrary.SteamInstallFolder, "steam.exe");

        if (!File.Exists(steamExecPath))
        {
            Logger.Log($"The Steam executable does not exist (at {steamExecPath}).");
            return;
        }

        Process.Start(steamExecPath, $"steam://rungameid/{AppId}");
    }
}
