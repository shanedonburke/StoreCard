#region

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

#endregion

namespace StoreCard.Utils;

/// <summary>
/// A class that detects theme files in the StoreCard installation folder.
/// </summary>
public static class ThemeFinder
{
    /// <summary>
    /// Matches absolute theme file paths, capturing the theme name.
    /// </summary>
    private static readonly Regex s_themePathRegex =
        new(@"^[a-zA-Z]:\\.+\\(?<themeName>.+)\.xaml$", RegexOptions.Compiled);

    /// <summary>
    /// Find all themes located in the StoreCard installation directory.
    /// </summary>
    /// <returns>List of theme names</returns>
    public static List<string> FindThemes()
    {
        string themeFolder = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
            @"ResourceDictionaries\Themes");

        IEnumerable<string> files = Directory.EnumerateFiles(
            themeFolder,
            "*.xaml",
            SearchOption.TopDirectoryOnly);

        var themeNames =
            (from file in files
                select s_themePathRegex.Match(file)
                into match
                where match.Success
                select match.Groups["themeName"].Value).ToList();

        return themeNames;
    }
}
