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
public abstract class InstalledGame : IListBoxItem
{
    /// <summary>
    /// Creates the game.
    /// </summary>
    /// <param name="name">Display name of the game</param>
    /// <param name="bitmapIcon">Icon to show for the game</param>
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

    /// <summary>
    /// Name of the game.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Icon to show for the game.
    /// </summary>
    public BitmapSource? BitmapIcon { get; }

    /// <summary>
    /// Icon to show next to the main icon, representing the type of item.
    /// </summary>
    public BitmapSource PrefixIcon => Icons.GameIcon;

    public int CompareTo(IListBoxItem? other) => string.Compare(Name, other?.Name, StringComparison.Ordinal);
}
