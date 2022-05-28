using System;
using Microsoft.Win32;

namespace StoreCard.Utils;

public class BrowserUtils
{
    private const string UserChoicePath =
        @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice";

    private const string ExeSuffix = ".exe";

    public static string? GetDefaultBrowserExecutable()
    {
        using RegistryKey? userChoiceKey = Registry.CurrentUser.OpenSubKey(UserChoicePath);
        string? progId = userChoiceKey?.GetValue("Progid")?.ToString();

        if (progId == null)
        {
            return null;
        }

        string openCommandPath = progId + @"\shell\open\command";
        using RegistryKey? openCommandPathKey = Registry.ClassesRoot.OpenSubKey(openCommandPath);

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
                // Get file path only
                executable =
                    executable[..(executable.LastIndexOf(ExeSuffix, StringComparison.Ordinal) + ExeSuffix.Length)];
            }
        }
        catch
        {
            Logger.Log("Failed to get the default browser's executable path. The registry value may be set incorrectly.");
        }

        return executable;
    }
}
