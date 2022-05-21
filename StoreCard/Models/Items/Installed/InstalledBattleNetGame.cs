using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using StoreCard.Models.Items.Saved;
using StoreCard.Static;

namespace StoreCard.Models.Items.Installed;

internal class InstalledBattleNetGame : InstalledGame
{
    public readonly string GameId;

    public InstalledBattleNetGame(string name, BitmapSource? bitmapIcon, string gameId) : base(name, bitmapIcon)
    {
        GameId = gameId;
    }

    public override string SecondaryText => GamePlatformNames.BattleNet;

    public override SavedItem SavedItem => new SavedBattleNetGame(this);
}
