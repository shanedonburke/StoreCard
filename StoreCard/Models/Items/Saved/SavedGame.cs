using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using StoreCard.Static;

namespace StoreCard.Models.Items.Saved;

internal abstract class SavedGame : SavedItem
{
    protected SavedGame(string id, string name, string? base64Icon) : base(id, name, base64Icon)
    {
    }

    public override ItemCategory Category => ItemCategory.Game;

    public override ImageSource PrefixIcon => Icons.GameIcon;
}