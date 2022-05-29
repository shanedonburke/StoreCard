#region

using System.Windows.Media.Imaging;
using StoreCard.Models.Items.Saved;
using StoreCard.Models.Items.Saved.Games;
using StoreCard.Static;

#endregion

namespace StoreCard.Models.Items.Installed.Games;

public sealed class InstalledItchGame : InstalledGame
{
    public readonly string CaveId;

    public InstalledItchGame(string name, BitmapSource? bitmapIcon, string caveId) : base(name, bitmapIcon) =>
        CaveId = caveId;

    public override string SecondaryText => GamePlatformNames.Itch;

    public override SavedItem SavedItem => new SavedItchGame(this);
}
