using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Shell;
using File = IWshRuntimeLibrary.File;

namespace StoreCard
{
    internal class SystemUtils
    {
        private static string StartupFolderPath => Environment.GetFolderPath(
            Environment.SpecialFolder.Startup);

        public static List<InstalledApplication> GetInstalledApplications()
        {
            // From https://stackoverflow.com/a/57195200
            var installedApps = new List<InstalledApplication>();
            var appsFolderId = new Guid("{1e87508d-89c2-42f0-8a7e-645a0f50ca58}");
            var appsFolder = (ShellObject)KnownFolderHelper.FromKnownFolderId(appsFolderId);

            foreach (var app in (IKnownFolder)appsFolder) {
                // The friendly app name
                var name = app.Name;
                // Path to executable
                var path = app.Properties.System.Link.TargetParsingPath.Value;
                // The ParsingName property is the AppUserModelID
                var appUserModelId = app.ParsingName;
                var icon = app.Thumbnail.SmallBitmapSource;
                icon.Freeze();

                installedApps.Add(new InstalledApplication(name, appUserModelId, path, icon));
            }
            installedApps.Sort();
            return installedApps;
        }

        public static void CreateDesktopShortcut()
        {
            CreateShortcut("Desktop");
        }

        public static void CreateStartupShortcut()
        {
            CreateShortcut(StartupFolderPath);
        }

        public static void RemoveStartupShortcut()
        {
            var shortcutPath = Path.Join(StartupFolderPath, "StoreCard.lnk");
            if (System.IO.File.Exists(shortcutPath))
            {
                System.IO.File.Delete(shortcutPath);
            }
        }

        private static void CreateShortcut(string folder)
        {
            var wshShell = new WshShell();

            // Create the shortcut
            var shortcut = (IWshShortcut)wshShell.CreateShortcut(
                Path.Join(folder, Application.ProductName + ".lnk"));

            shortcut.TargetPath = Application.ExecutablePath;
            shortcut.WorkingDirectory = Application.StartupPath;
            shortcut.Description = "Launch StoreCard";
            shortcut.Save();
        }
    }
}
