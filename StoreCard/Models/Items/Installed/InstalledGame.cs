using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StoreCard.Models.Items.Saved;
using static System.String;

namespace StoreCard.Models.Items.Installed;

public abstract class InstalledGame : IComparable<InstalledGame>, IListBoxItem
{
    protected InstalledGame(string name, BitmapSource bitmapIcon)
    {
        Name = name;
        BitmapIcon = bitmapIcon;
    }

    public string Name { get; }

    public ImageSource BitmapIcon { get; }

    public abstract ImageSource? PrefixIcon { get; }

    public abstract SavedItem SavedItem { get; }

    public int CompareTo(InstalledGame? other)
    {
        return Compare(Name, other?.Name, StringComparison.Ordinal);
    }
}