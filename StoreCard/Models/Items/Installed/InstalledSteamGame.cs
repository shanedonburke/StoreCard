using System.Windows.Media;
using StoreCard.Models.Items.Saved;
using StoreCard.Static;

namespace StoreCard.Models.Items.Installed;

public class InstalledSteamGame : InstalledGame
{
    public InstalledSteamGame(
        string name,
        ImageSource bitmapIcon,
        string appId) : base(name, bitmapIcon)
    {
        AppId = appId;
    }

    public override string SecondaryText => GamePlatformNames.Steam;

    public string AppId { get; }

    public override SavedItem SavedItem => new SavedSteamGame(this);
}