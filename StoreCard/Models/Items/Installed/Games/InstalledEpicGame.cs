#region

using System.Windows.Media.Imaging;
using StoreCard.Models.Items.Saved;
using StoreCard.Models.Items.Saved.Games;
using StoreCard.Static;

#endregion

namespace StoreCard.Models.Items.Installed.Games;

/// <summary>
/// Represents an installed Epic Games game.
/// </summary>
public sealed class InstalledEpicGame : InstalledGame
{
    /// <summary>
    /// App name used to launch the game.
    /// </summary>
    public readonly string AppName;

    /// <summary>
    /// Creates the game.
    /// </summary>
    /// <param name="name">Game name</param>
    /// <param name="bitmapIcon">Game icon</param>
    /// <param name="appName">App name used to launch the game</param>
    public InstalledEpicGame(
        string name,
        BitmapSource bitmapIcon,
        string appName) : base(name,
        bitmapIcon) =>
        AppName = appName;

    public override string SecondaryText => GamePlatformNames.EpicGames;

    public override SavedItem SavedItem => new SavedEpicGame(this);
}
