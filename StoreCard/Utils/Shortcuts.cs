using System;
using System.IO;
using System.Windows.Forms;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using StoreCard.Static;
using File = System.IO.File;

namespace StoreCard.Utils;

internal class Shortcuts
{
    private static string StartupFolderPath => Environment.GetFolderPath(
        Environment.SpecialFolder.Startup);

    private static string StartupShortcutPath => Path.Join(StartupFolderPath, "StoreCard.lnk");

    public static void CreateStartupShortcut() {
        CreateShortcut(StartupFolderPath, CommandLineArgs.StartMinimized);
    }

    public static void RemoveStartupShortcut() {
        if (File.Exists(StartupShortcutPath)) {
            File.Delete(StartupShortcutPath);
        }
    }

    public static bool? IsStartupShortcutEnabled() {
        if (!File.Exists(StartupShortcutPath)) {
            return null;
        }

        var regKey = Registry.CurrentUser.OpenSubKey
            (@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\StartupFolder", true);

        if (regKey?.GetValue("StoreCard.lnk") is byte[] { Length: > 0 } regValue) {
            return regValue[0] == 0x2;
        }

        return null;
    }

    private static void CreateShortcut(string folder, string arguments = "") {
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