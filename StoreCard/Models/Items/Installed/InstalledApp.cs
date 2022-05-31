#region

using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;
using StoreCard.Models.Items.Saved;
using StoreCard.Static;

#endregion

namespace StoreCard.Models.Items.Installed;

/// <summary>
/// Represents an installed app or Xbox game.
/// </summary>
public sealed class InstalledApp : IListBoxItem, IInstalledItem
{
    public InstalledApp(string name, string appUserModelId, string? executablePath, BitmapSource icon)
    {
        Name = name;
        AppUserModelId = appUserModelId;
        ExecutablePath = executablePath;
        BitmapIcon = icon;
        IsBattleNetGame = CheckIfBattleNetGame();
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

    /// <summary>
    /// Windows detects Battle.net games as apps, so we represent them as one
    /// with this flag.
    /// </summary>
    public bool IsBattleNetGame { get; }

    public string SecondaryText => IsBattleNetGame ? GamePlatformNames.BattleNet : string.Empty;

    public string Name { get; }

    public BitmapSource BitmapIcon { get; }

    public BitmapSource PrefixIcon => Icons.AppIcon;

    public SavedItem SavedItem => new SavedApp(this);

    public int CompareTo(IListBoxItem? other) => string.Compare(Name, other?.Name, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Check if this item represents a Battle.net game. Windows detects these games as apps, so it's
    /// convenient to represent them using this class.
    /// </summary>
    /// <returns><c>true</c> if this item is a Battle.net game</returns>
    private bool CheckIfBattleNetGame()
    {
        // For Battle.net games, the AUMID is the path to the executable
        if (File.Exists(AppUserModelId))
        {
            // The "Copyright" property will include the string "Blizzard Entertainment"
            string? copyright = FileVersionInfo.GetVersionInfo(AppUserModelId).LegalCopyright;

            if (copyright?.Contains("Blizzard Entertainment") == true)
            {
                return true;
            }
        }

        return false;
    }
}
