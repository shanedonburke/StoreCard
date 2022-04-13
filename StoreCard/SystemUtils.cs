using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.WindowsAPICodePack.Shell;

namespace StoreCard
{
    internal class SystemUtils
    {
        public static void OpenInDefaultProgram(string path)
        {
            using Process openProcess = new Process();

            openProcess.StartInfo.FileName = "explorer.exe";
            openProcess.StartInfo.Arguments = $"\"{path}\"";
            openProcess.Start();
        }

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

            return installedApps;
        }
    }
}
