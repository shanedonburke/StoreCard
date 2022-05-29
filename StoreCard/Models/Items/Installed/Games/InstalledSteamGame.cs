#region

using System.Windows.Media.Imaging;
using StoreCard.Models.Items.Saved;
using StoreCard.Models.Items.Saved.Games;
using StoreCard.Static;

#endregion

namespace StoreCard.Models.Items.Installed.Games;

public sealed class InstalledSteamGame : InstalledGame
{
    public readonly string AppId;

    public InstalledSteamGame(string name, BitmapSource bitmapIcon, string appId) : base(name, bitmapIcon) =>
        AppId = appId;

    public override string SecondaryText => GamePlatformNames.Steam;

    public override SavedItem SavedItem => new SavedSteamGame(this);
}
