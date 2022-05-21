using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using StoreCard.GameLibraries;
using StoreCard.GameLibraries.Steam;
using StoreCard.Models.Items.Installed;
using StoreCard.Static;
using StoreCard.Utils;

namespace StoreCard.Models.Items.Saved;

internal class SavedSteamGame : SavedGame
{
    [JsonConstructor]
    public SavedSteamGame(
        string id,
        string name,
        string base64Icon,
        string appId,
        long lastOpened) : base(id, name, base64Icon, lastOpened)
    {
        AppId = appId;
    }

    public SavedSteamGame(InstalledSteamGame game) : base(
        Guid.NewGuid().ToString(),
        game.Name,
        game.BitmapIcon?.ToBase64(),
        Time.UnixTimeMillis
    )
    {
        AppId = game.AppId;
    }

    public string AppId { get; }

    public override SpecificItemCategory SpecificCategory => SpecificItemCategory.SteamGame;

    public override string SecondaryText => GamePlatformNames.Steam;

    protected override void OpenProtected()
    {
        if (SteamLibrary.SteamInstallFolder == null)
        {
            MessageBox.Show("The Steam installation folder could not be found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        string steamExecPath = Path.Combine(SteamLibrary.SteamInstallFolder, "steam.exe");
        if (!File.Exists(steamExecPath)) return;

        Process.Start(steamExecPath, $"steam://rungameid/{AppId}");
    }
}
