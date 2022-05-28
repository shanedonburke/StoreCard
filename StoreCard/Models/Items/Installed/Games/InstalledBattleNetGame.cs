using System.Windows.Media.Imaging;
using StoreCard.Models.Items.Saved;
using StoreCard.Models.Items.Saved.Games;
using StoreCard.Static;

namespace StoreCard.Models.Items.Installed.Games;

internal sealed class InstalledBattleNetGame : InstalledGame
{
    public readonly string GameId;

    public InstalledBattleNetGame(string name, BitmapSource? bitmapIcon, string gameId) : base(name, bitmapIcon)
    {
        GameId = gameId;
    }

    public override string SecondaryText => GamePlatformNames.BattleNet;

    public override SavedItem SavedItem => new SavedBattleNetGame(this);
}
