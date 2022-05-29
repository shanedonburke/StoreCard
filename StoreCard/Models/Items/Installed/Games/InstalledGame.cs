#region

using System;
using System.Windows.Media.Imaging;
using StoreCard.Models.Items.Saved;
using StoreCard.Static;

#endregion

namespace StoreCard.Models.Items.Installed.Games;

public abstract class InstalledGame : IListBoxItem
{
    protected InstalledGame(string name, BitmapSource? bitmapIcon)
    {
        Name = name;
        BitmapIcon = bitmapIcon;
    }

    public abstract SavedItem SavedItem { get; }

    public virtual string SecondaryText => string.Empty;

    public string Name { get; }

    public BitmapSource? BitmapIcon { get; }

    public BitmapSource PrefixIcon => Icons.GameIcon;

    public int CompareTo(IListBoxItem? other) => string.Compare(Name, other?.Name, StringComparison.Ordinal);
}
