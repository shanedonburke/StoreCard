using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using StoreCard.GameLibraries;
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
        string appId) : base(id, name, base64Icon)
    {
        AppId = appId;
    }

    public SavedSteamGame(InstalledSteamGame game) : base(
        Guid.NewGuid().ToString(),
        game.Name,
        Images.ImageToBase64((BitmapSource) game.BitmapIcon)
    )
    {
        AppId = game.AppId;
    }

    public string AppId { get; }

    public override SpecificItemCategory SpecificCategory => SpecificItemCategory.SteamGame;

    public override string SecondaryText => GamePlatformNames.Steam;

    public override void Open()
    {
        if (SteamLibrary.SteamInstallFolder == null)
        {
            MessageBox.Show("The Steam installation folder could not be found.");
            return;
        }

        var steamExecPath = Path.Combine(SteamLibrary.SteamInstallFolder, "steam.exe");
        Process.Start(steamExecPath, $"steam://rungameid/{AppId}");
    }
}