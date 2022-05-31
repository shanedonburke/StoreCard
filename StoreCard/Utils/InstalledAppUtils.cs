#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using Microsoft.WindowsAPICodePack.Shell;
using StoreCard.Models.Items.Installed;

#endregion

namespace StoreCard.Utils;

/// <summary>
/// Utilties for getting the applications and application-like games installed on the system.
/// </summary>
public static class InstalledAppUtils
{
    /// <summary>
    /// GUID for the virtual "apps" folder.
    /// </summary>
    private static readonly Guid s_appsFolderId = new("{1e87508d-89c2-42f0-8a7e-645a0f50ca58}");

    /// <summary>
    /// Gets the installed apps, minus any non-Xbox games. Xbox games are hard to distinguish
    /// from other applications, so they're listed alongside them.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<InstalledApp> GetInstalledApps()
    {
        return _getInstalledApps().Where(a => !a.IsBattleNetGame);
    }

    /// <summary>
    /// Gets the list of installed Battle.net games. Battle.net games are detected as
    /// applications, so they're obtained in the same way.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<InstalledApp> GetBattleNetGames()
    {
        return _getInstalledApps().Where(a => a.IsBattleNetGame);
    }

    /// <summary>
    /// Gets the list of installed applications and application-like games.
    /// See <see href="https://stackoverflow.com/a/57195200">this post</see> for more info.
    /// </summary>
    /// <returns></returns>
    private static IEnumerable<InstalledApp> _getInstalledApps()
    {
        var appsFolder = (ShellObject)KnownFolderHelper.FromKnownFolderId(s_appsFolderId);

        foreach (ShellObject? app in (IKnownFolder)appsFolder)
        {
            // The friendly app name
            string? name = app.Name;
            // Path to executable
            string? path = app.Properties.System.Link.TargetParsingPath.Value;
            // The ParsingName property is the AppUserModelID
            string? appUserModelId = app.ParsingName;
            BitmapSource? icon = app.Thumbnail.SmallBitmapSource;
            // Must freeze icon to access in other threads
            icon.Freeze();

            yield return new InstalledApp(name, appUserModelId, path, icon);
        }
    }
}
