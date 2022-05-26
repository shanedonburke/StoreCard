using System.Windows.Media.Imaging;
using StoreCard.Static;

namespace StoreCard.Models.Items.Saved.Games;

internal abstract class SavedGame : SavedItem
{
    protected SavedGame(string id, string name, string? base64Icon, long lastOpened) : base(id, name, base64Icon,
        lastOpened)
    {
    }

    public override ItemCategory Category => ItemCategory.Game;

    public override BitmapSource PrefixIcon => Icons.GameIcon;
}
