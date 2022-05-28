using System;
using System.Windows.Media.Imaging;

namespace StoreCard.Models.Items.Installed;

internal sealed class InstalledApp : IListBoxItem
{
    public InstalledApp(string name, string appUserModelId, string? executablePath, BitmapSource icon)
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

    public BitmapSource BitmapIcon { get; }

    public BitmapSource? PrefixIcon => null;

    public int CompareTo(IListBoxItem? other)
    {
        return string.Compare(Name, other?.Name, StringComparison.OrdinalIgnoreCase);
    }
}
