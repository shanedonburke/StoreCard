using System;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using StoreCard.Models.Items.Installed;
using StoreCard.Static;
using StoreCard.Utils;

namespace StoreCard.Models.Items.Saved;

internal class SavedEpicGame : SavedItem
{
    [JsonConstructor]
    public SavedEpicGame(
        string id,
        string name,
        string base64Icon,
        string appName) : base(id, name, base64Icon)
    {
        AppName = appName;
    }

    public SavedEpicGame(InstalledEpicGame game) : base(
        Guid.NewGuid().ToString(),
        game.Name,
        ImageUtils.ImageToBase64((BitmapSource)game.BitmapIcon)
    )
    {
        AppName = game.AppName;
    }

    public string AppName { get;  }

    public override ItemCategory Category => ItemCategory.Game;

    public override ImageSource PrefixIcon => Icons.GameIcon;

    public override void Open()
    {
        Process.Start("CMD.exe", $"/c START com.epicgames.launcher://apps/{AppName}?action=launch&silent=true ");
    }
}