using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StoreCard.Models.Items.Installed;

public class InstalledApplication : IListBoxItem
{
    public InstalledApplication(string name, string appUserModelId, string? executablePath, BitmapSource icon)
    {
        Name = name;
        AppUserModelId = appUserModelId;
        ExecutablePath = executablePath;
        BitmapIcon = icon;
    }

    public string SecondaryText => string.Empty;

    public string Name { get; }
    public string AppUserModelId { get; }
    public string? ExecutablePath { get; }
    public ImageSource BitmapIcon { get; }
    public ImageSource? PrefixIcon => null;

    public int CompareTo(IListBoxItem? other) {
        return string.Compare(Name, other?.Name, StringComparison.OrdinalIgnoreCase);
    }
}