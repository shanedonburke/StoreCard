#region

using System;
using System.Windows.Media.Imaging;
using StoreCard.Static;

#endregion

namespace StoreCard.Models.Items.Installed;

/// <summary>
/// Represents an installed app or Xbox game.
/// </summary>
public sealed class InstalledApp : IListBoxItem
{
    public InstalledApp(string name, string appUserModelId, string? executablePath, BitmapSource icon)
    {
        Name = name;
        AppUserModelId = appUserModelId;
        ExecutablePath = executablePath;
        BitmapIcon = icon;
    }

    /// <summary>
    /// AUMID used to launch the app.
    /// </summary>
    public string AppUserModelId { get; }

    /// <summary>
    /// Path to the app's executable. Used when an installed app
    /// is selected as the executable for another item.
    /// </summary>
    public string? ExecutablePath { get; }

    public string SecondaryText => string.Empty;

    public string Name { get; }

    public BitmapSource BitmapIcon { get; }

    public BitmapSource PrefixIcon => Icons.AppIcon;

    public int CompareTo(IListBoxItem? other) => string.Compare(Name, other?.Name, StringComparison.OrdinalIgnoreCase);
}
