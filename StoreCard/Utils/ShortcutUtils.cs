#region

using System.IO;
using System.Windows.Forms;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using StoreCard.Static;
using File = System.IO.File;

#endregion

namespace StoreCard.Utils;

/// <summary>
/// Utilities for working with system shortcut files.
/// </summary>
public static class ShortcutUtils
{
    /// <summary>
    /// Path to the StoreCard startup shortcut.
    /// </summary>
    private static string StartupShortcutPath => Path.Join(FolderPaths.Startup, "StoreCard.lnk");

    /// <summary>
    /// Create a shortcut file that enables StoreCard to launch on system startup,
    /// assuming it isn't disabled in Task Manager or elsewhere. StoreCard
    /// will start without opening the main window in this case.
    /// </summary>
    public static void CreateStartupShortcut() =>
        CreateShortcut(FolderPaths.Startup, CommandLineOptions.StartMinimized);

    /// <summary>
    /// Remove the startup shortcut file for StoreCard, if there is one.
    /// </summary>
    public static void RemoveStartupShortcut()
    {
        if (File.Exists(StartupShortcutPath))
        {
            File.Delete(StartupShortcutPath);
        }
    }

    /// <summary>
    /// Check if StoreCard is enabled on system startup.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the shortcut exists and is enabled,
    /// <c>false</c> if a disabled shortcut exists,
    /// <c>null</c> if no shortcut exists
    /// </returns>
    public static bool? IsStartupShortcutEnabled()
    {
        if (!File.Exists(StartupShortcutPath))
        {
            return null;
        }

        // Registry location for startup shortcuts
        RegistryKey? regKey = Registry.CurrentUser.OpenSubKey(RegUtils.Paths.StartupFolder64, true);

        if (regKey?.GetValue("StoreCard.lnk") is byte[] {Length: > 0} regValue)
        {
            // 0x2 indicates the shortcut hasn't been disabled
            return regValue[0] == 0x2;
        }

        return null;
    }

    /// <summary>
    /// Create a StoreCard shortcut file in the given folder.
    /// </summary>
    /// <param name="folder">Destination folder</param>
    /// <param name="arguments">Arguments to add</param>
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
