#region

using StoreCard.Models.Items.Saved;

#endregion

namespace StoreCard.Models.Items.Installed;

/// <summary>
/// Represents an installed app or game.
/// </summary>
internal interface IInstalledItem
{
    /// <summary>
    /// The <see cref="SavedItem"/> representation of this item.
    /// </summary>
    public SavedItem SavedItem { get; }
}
