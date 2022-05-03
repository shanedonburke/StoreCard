using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StoreCard.Models.Items.Saved;
using StoreCard.Static;
using static System.String;

namespace StoreCard.Models.Items.Installed;

public abstract class InstalledGame : IComparable<InstalledGame>, IListBoxItem
{
    protected InstalledGame(string name, ImageSource bitmapIcon)
    {
        Name = name;
        BitmapIcon = bitmapIcon;
    }

    public string Name { get; }

    public ImageSource BitmapIcon { get; }

    public ImageSource PrefixIcon => Icons.GameIcon;

    public abstract SavedItem SavedItem { get; }

    public int CompareTo(InstalledGame? other)
    {
        return Compare(Name, other?.Name, StringComparison.Ordinal);
    }
}