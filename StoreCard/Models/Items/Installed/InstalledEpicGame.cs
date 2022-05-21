using System.Windows.Media;
using System.Windows.Media.Imaging;
using StoreCard.Models.Items.Saved;
using StoreCard.Static;

namespace StoreCard.Models.Items.Installed;

internal class InstalledEpicGame : InstalledGame
{
    public readonly string AppName;

    public InstalledEpicGame(
        string name,
        BitmapSource bitmapIcon,
        string appName) : base(name, bitmapIcon)
    {
        AppName = appName;
    }

    public override string SecondaryText => GamePlatformNames.EpicGames;

    public override SavedItem SavedItem => new SavedEpicGame(this);
}
