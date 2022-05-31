#region

using System;
using Microsoft.Win32;

#endregion

namespace StoreCard.Utils;

/// <summary>
/// Utilities for detecting the system's default browser.
/// </summary>
public class BrowserUtils
{
    /// <summary>
    /// Registry key where the default browser can be found.
    /// </summary>
    private const string UserChoicePath =
        @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice";

    private const string ExeSuffix = ".exe";

    /// <summary>
    /// Gets the .exe file for the default browser.
    /// </summary>
    /// <returns></returns>
    public static string? GetDefaultBrowserExecutable()
    {
        using RegistryKey? userChoiceKey = Registry.CurrentUser.OpenSubKey(UserChoicePath);
        string? progId = userChoiceKey?.GetValue("Progid")?.ToString();

        if (progId == null)
        {
            return null;
        }

        // Key whose value is the browser executable
        using RegistryKey? openCommandPathKey = Registry.ClassesRoot.OpenSubKey(progId + @"\shell\open\command");

        if (openCommandPathKey == null)
        {
            return null;
        }

        string? executable = null;

        // The value may contain the exe path plus arguments
        try
        {
            executable = openCommandPathKey.GetValue(null)?.ToString()?.ToLower().Replace("\"", "");

            if (executable != null && !executable.EndsWith(ExeSuffix))
            {
                // Get file path only (no arguments)
                executable =
                    executable[..(executable.LastIndexOf(ExeSuffix, StringComparison.Ordinal) + ExeSuffix.Length)];
            }
        }
        catch
        {
            Logger.Log(
                "Failed to get the default browser's executable path. The registry value may be set incorrectly.");
        }

        return executable;
    }
}
