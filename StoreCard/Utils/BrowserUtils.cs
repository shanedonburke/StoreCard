using System;
using Microsoft.Win32;

namespace StoreCard.Utils;

internal class BrowserUtils
{
    private const string UserChoicePath = @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice";
    private const string ExeSuffix = ".exe";

    public static string? GetDefaultBrowserExecutable()
    {
        using var userChoiceKey = Registry.CurrentUser.OpenSubKey(UserChoicePath);
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

            if (executable != null && !executable.EndsWith(ExeSuffix))
            {
                executable = executable[..(executable.LastIndexOf(ExeSuffix, StringComparison.Ordinal) + ExeSuffix.Length)];
            }
        }
        catch
        {
            // Assume the registry value is set incorrectly
        }

        return executable;
    }
}
