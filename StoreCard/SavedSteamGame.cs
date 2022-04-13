using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Newtonsoft.Json;

namespace StoreCard;

public class SavedSteamGame : SavedItem
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
        ImageUtils.ImageToBase64(game.BitmapIcon)
    )
    {
        AppId = game.AppId;
    }

    public string AppId { get; }

    public override ItemCategory Category => ItemCategory.Game;

    public override ImageSource PrefixIcon => GamePlatformIcons.SteamIcon;

    public override void Open()
    {
        if (Paths.SteamInstallFolder == null)
        {
            MessageBox.Show("The Steam installation folder could not be found.");
            return;
        }

        var steamExecPath = Path.Combine(Paths.SteamInstallFolder, "steam.exe");
        Process.Start(steamExecPath, $"steam://rungameid/{AppId}");
    }
}