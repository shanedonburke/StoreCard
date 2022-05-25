using System;
using Microsoft.Win32;

namespace StoreCard.Utils;

internal class BrowserUtils
{
    public static string? GetDefaultBrowserExecutable()
    {
        const string userChoicePath = @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice";
        const string exeSuffix = ".exe";

        using var userChoiceKey = Registry.CurrentUser.OpenSubKey(userChoicePath);

        string? progId = userChoiceKey?.GetValue("Progid")?.ToString();
        if (progId == null)
        {
            return null;
        }

        string openCommandPath = progId + @"\shell\open\command";

        using var openCommandPathKey = Registry.ClassesRoot.OpenSubKey(openCommandPath);

        if (openCommandPathKey == null)
        {
            return null;
        }

        string? executable = null;

        // Trim parameters
        try
        {
            executable = openCommandPathKey.GetValue(null)?.ToString()?.ToLower().Replace("\"", "");

            if (executable != null && !executable.EndsWith(exeSuffix))
            {
                executable = executable[..(executable.LastIndexOf(exeSuffix, StringComparison.Ordinal) + exeSuffix.Length)];
            }
        }
        catch
        {
            // Assume the registry value is set incorrectly
        }

        return executable;
    }
}
