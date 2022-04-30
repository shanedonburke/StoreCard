using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Shell;
using File = System.IO.File;

namespace StoreCard
{
    internal class SystemUtils
    {
        private static string StartupFolderPath => Environment.GetFolderPath(
            Environment.SpecialFolder.Startup);

        private static string StartupShortcutPath => Path.Join(StartupFolderPath, "StoreCard.lnk");

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

        public static void CreateStartupShortcut()
        {
            CreateShortcut(StartupFolderPath, CommandLineArgs.StartMinimized);
        }

        public static void RemoveStartupShortcut()
        {
            if (File.Exists(StartupShortcutPath))
            {
                File.Delete(StartupShortcutPath);
            }
        }

        public static bool? IsStartupShortcutEnabled()
        {
            if (!File.Exists(StartupShortcutPath))
            {
                return null;
            }

            var regKey = Registry.CurrentUser.OpenSubKey
                (@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\StartupFolder", true);

            if (regKey?.GetValue("StoreCard.lnk") is byte[] { Length: > 0 } regValue)
            {
                return regValue[0] == 0x2;
            }

            return null;
        }

        private static void CreateShortcut(string folder, string arguments = "")
        {
            var wshShell = new WshShell();

            // Create the shortcut
            var shortcut = (IWshShortcut)wshShell.CreateShortcut(
                Path.Join(folder, Application.ProductName + ".lnk"));

            shortcut.TargetPath = Application.ExecutablePath;
            shortcut.Arguments = arguments;
            shortcut.WorkingDirectory = Application.StartupPath;
            shortcut.Description = "Launch StoreCard";
            shortcut.Save();
        }
    }
}
