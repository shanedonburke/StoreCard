using System.Windows.Media.Imaging;
using StoreCard.Models.Items.Saved;
using StoreCard.Models.Items.Saved.Games;
using StoreCard.Static;

namespace StoreCard.Models.Items.Installed.Games;

public sealed class InstalledEaGame : InstalledGame
{
    public readonly string AppId;

    public InstalledEaGame(string name, BitmapSource? bitmapIcon, string appId) : base(name, bitmapIcon)
    {
        AppId = appId;
    }

    public override SavedItem SavedItem => new SavedEaGame(this);

    public override string SecondaryText => GamePlatformNames.Ea;
}
