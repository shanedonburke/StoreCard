#region

using System.Windows.Media.Imaging;
using StoreCard.Models.Items.Saved;
using StoreCard.Models.Items.Saved.Games;
using StoreCard.Static;

#endregion

namespace StoreCard.Models.Items.Installed.Games;

/// <summary>
/// Represents an installed EA (EA Desktop or Origin) game.
/// </summary>
public sealed class InstalledEaGame : InstalledGame
{
    /// <summary>
    /// App ID used to launch the game.
    /// </summary>
    public readonly string AppId;

    public InstalledEaGame(
        string name,
        BitmapSource? bitmapIcon,
        string appId) : base(name,
        bitmapIcon) =>
        AppId = appId;

    public override SavedItem SavedItem => new SavedEaGame(this);

    public override string SecondaryText => GamePlatformNames.Ea;
}
