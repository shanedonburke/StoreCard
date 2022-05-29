#region

using System;
using System.Windows.Media.Imaging;

#endregion

namespace StoreCard.Models.Items.Installed;

public sealed class InstalledApp : IListBoxItem
{
    public InstalledApp(string name, string appUserModelId, string? executablePath, BitmapSource icon)
    {
        Name = name;
        AppUserModelId = appUserModelId;
        ExecutablePath = executablePath;
        BitmapIcon = icon;
    }

    public string AppUserModelId { get; }

    public string? ExecutablePath { get; }

    public string SecondaryText => string.Empty;

    public string Name { get; }

    public BitmapSource BitmapIcon { get; }

    public BitmapSource? PrefixIcon => null;

    public int CompareTo(IListBoxItem? other) => string.Compare(Name, other?.Name, StringComparison.OrdinalIgnoreCase);
}
