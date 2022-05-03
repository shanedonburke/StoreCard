using System.Windows.Media;
using System.Windows.Media.Imaging;
using StoreCard.Models.Items.Saved;
using StoreCard.Static;

namespace StoreCard.Models.Items.Installed;

public class InstalledSteamGame : InstalledGame
{
    public InstalledSteamGame(
        string name,
        string appId,
        ImageSource bitmapIcon) : base(name, bitmapIcon)
    {
        AppId = appId;
    }

    public string AppId { get; }

    public override BitmapSource PrefixIcon => Icons.SteamIcon;

    public override SavedItem SavedItem => new SavedSteamGame(this);
}