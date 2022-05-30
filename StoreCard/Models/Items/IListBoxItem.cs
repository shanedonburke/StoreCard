#region

using System;
using System.Windows.Media.Imaging;

#endregion

namespace StoreCard.Models.Items;

/// <summary>
/// Represents an item to be displayed in a <c>StoreCardListBox</c>.
/// </summary>
public interface IListBoxItem : IComparable<IListBoxItem>
{
    /// <summary>
    /// Name/main item text.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Main icon.
    /// </summary>
    public BitmapSource? BitmapIcon { get; }

    /// <summary>
    /// Optional secondary icon to show next to the main icon.
    /// </summary>
    public BitmapSource? PrefixIcon { get; }

    /// <summary>
    /// secondary text to show for the item. Can be an empty string.
    /// </summary>
    public string SecondaryText { get; }
}
