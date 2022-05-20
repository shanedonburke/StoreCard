using System.Windows.Media;
using StoreCard.Static;

namespace StoreCard.Models.Items.Saved;

internal abstract class SavedGame : SavedItem
{
    protected SavedGame(string id, string name, string? base64Icon, long lastOpened) : base(id, name, base64Icon,
        lastOpened)
    {
    }

    public override ItemCategory Category => ItemCategory.Game;

    public override ImageSource PrefixIcon => Icons.GameIcon;
}
