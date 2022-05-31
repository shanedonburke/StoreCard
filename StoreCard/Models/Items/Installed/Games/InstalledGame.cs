#region

using System;
using System.Windows.Media.Imaging;
using StoreCard.Models.Items.Saved;
using StoreCard.Static;

#endregion

namespace StoreCard.Models.Items.Installed.Games;

/// <summary>
/// Represents an installed game to be displayed when selecting a new game to store.
/// </summary>
public abstract class InstalledGame : IListBoxItem, IInstalledItem
{
    protected InstalledGame(string name, BitmapSource? bitmapIcon)
    {
        Name = name;
        BitmapIcon = bitmapIcon;
    }

    /// <summary>
    /// Getter for the <see cref="SavedItem"/> analogue to this game.
    /// </summary>
    public abstract SavedItem SavedItem { get; }

    /// <summary>
    /// Text to show next to this game, i.e., the name of the launcher.
    /// </summary>
    public virtual string SecondaryText => string.Empty;

    public string Name { get; }

    public BitmapSource? BitmapIcon { get; }

    public BitmapSource PrefixIcon => Icons.GameIcon;

    public int CompareTo(IListBoxItem? other) => string.Compare(Name, other?.Name, StringComparison.OrdinalIgnoreCase);
}
