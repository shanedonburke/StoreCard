using System;
using System.Windows.Media;
using StoreCard.Models.Items.Saved;
using StoreCard.Static;

namespace StoreCard.Models.Items.Installed;

public abstract class InstalledGame : IListBoxItem
{
    protected InstalledGame(string name, ImageSource bitmapIcon)
    {
        Name = name;
        BitmapIcon = bitmapIcon;
    }

    public virtual string SecondaryText => string.Empty;

    public string Name { get; }

    public ImageSource BitmapIcon { get; }

    public ImageSource PrefixIcon => Icons.GameIcon;

    public abstract SavedItem SavedItem { get; }

    public int CompareTo(IListBoxItem? other) {
        return string.Compare(Name, other?.Name, StringComparison.Ordinal);
    }
}