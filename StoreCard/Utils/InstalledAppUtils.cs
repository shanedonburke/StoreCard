using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using Microsoft.WindowsAPICodePack.Shell;
using StoreCard.Models.Items.Installed;

namespace StoreCard.Utils;

public class InstalledAppUtils
{
    private static readonly Guid s_appsFolderId = new("{1e87508d-89c2-42f0-8a7e-645a0f50ca58}");

    public static IEnumerable<InstalledApp> GetInstalledApps()
    {
        // From https://stackoverflow.com/a/57195200
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
            icon.Freeze();

            yield return new InstalledApp(name, appUserModelId, path, icon);
        }
    }
}
