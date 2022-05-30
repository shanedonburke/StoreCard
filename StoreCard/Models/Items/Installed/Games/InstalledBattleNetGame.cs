#region

using System.Windows.Media.Imaging;
using StoreCard.Models.Items.Saved;
using StoreCard.Models.Items.Saved.Games;
using StoreCard.Static;

#endregion

namespace StoreCard.Models.Items.Installed.Games;

/// <summary>
/// Represents an installed Battle.net game.
/// </summary>
public sealed class InstalledBattleNetGame : InstalledGame
{
    /// <summary>
    /// Game ID used to launch the game.
    /// </summary>
    public readonly string GameId;

    public InstalledBattleNetGame(
        string name,
        BitmapSource? bitmapIcon,
        string gameId) : base(name,
        bitmapIcon) =>
        GameId = gameId;

    public override string SecondaryText => GamePlatformNames.BattleNet;

    public override SavedItem SavedItem => new SavedBattleNetGame(this);
}
