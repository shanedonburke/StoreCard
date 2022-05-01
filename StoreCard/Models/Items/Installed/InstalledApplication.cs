using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static System.String;

namespace StoreCard.Models.Items.Installed;

public class InstalledApplication : IComparable<InstalledApplication>, IListBoxItem
{
    public InstalledApplication(string name, string appUserModelId, string? executablePath, BitmapSource icon)
    {
        Name = name;
        AppUserModelId = appUserModelId;
        ExecutablePath = executablePath;
        BitmapIcon = icon;
    }

    public string Name { get; }
    public string AppUserModelId { get; }
    public string? ExecutablePath { get; }
    public ImageSource BitmapIcon { get; }
    public ImageSource? PrefixIcon => null;

    public int CompareTo(InstalledApplication? other)
    {
        return Compare(Name, other?.Name, StringComparison.Ordinal);
    }
}