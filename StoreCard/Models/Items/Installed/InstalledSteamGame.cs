using System.Windows.Media;
using System.Windows.Media.Imaging;
using StoreCard.Models.Items.Saved;
using StoreCard.Static;

namespace StoreCard.Models.Items.Installed;

public class InstalledSteamGame : InstalledGame
{
    public readonly string AppId;

    public InstalledSteamGame(
        string name,
        BitmapSource bitmapIcon,
        string appId) : base(name, bitmapIcon)
    {
        AppId = appId;
    }

    public override string SecondaryText => GamePlatformNames.Steam;

    public override SavedItem SavedItem => new SavedSteamGame(this);
}
