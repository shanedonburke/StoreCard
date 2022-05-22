using System.Windows.Media.Imaging;
using StoreCard.Models.Items.Saved;
using StoreCard.Static;

namespace StoreCard.Models.Items.Installed;

internal class InstalledEaGame : InstalledGame
{
    public readonly string AppId;

    public InstalledEaGame(string name, string appId, BitmapSource? bitmapIcon) : base(name, bitmapIcon)
    {
        AppId = appId;
    }

    public override SavedItem SavedItem => new SavedEaGame(this);

    public override string SecondaryText => GamePlatformNames.Ea;
}
