using System.Windows.Media.Imaging;
using StoreCard.Models.Items.Saved;
using StoreCard.Static;

namespace StoreCard.Models.Items.Installed;

internal class InstalledEaGame : InstalledGame
{
    public readonly string GameId;

    public InstalledEaGame(string name, string gameId, BitmapSource? bitmapIcon) : base(name, bitmapIcon)
    {
        GameId = gameId;
    }

    public override SavedItem SavedItem => new SavedEaGame(this);

    public override string SecondaryText => GamePlatformNames.Ea;
}
