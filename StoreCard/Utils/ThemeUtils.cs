#region

using System.IO;
using System.Windows;

#endregion

namespace StoreCard.Utils;

/// <summary>
/// Utilities for working with themes.
/// </summary>
public static class ThemeUtils
{
    /// <summary>
    /// The default theme name.
    /// </summary>
    public const string DefaultThemeName = "Deep Sea (Dark)";

    /// <summary>
    /// Set the theme to the file with the given name.
    /// E.g., pass in "Lake (Dark)" for "Lake (Dark).xaml".
    /// If the theme isn't found, a default is used.
    /// </summary>
    /// <param name="theme">Theme name without path or extension</param>
    public static void SetTheme(string theme)
    {
        if (SetThemeInternal(theme))
        {
            return;
        }

        // Default to Deep Sea (Dark)
        if (!SetThemeInternal(DefaultThemeName))
        {
            SetThemeInternal(ThemeFinder.FindThemes()[0]);
        }
    }

    /// <summary>
    /// Set the theme to the file with the given name.
    /// E.g., pass in "Lake (Dark)" for "Lake (Dark).xaml".
    /// </summary>
    /// <param name="theme">Theme name without path or extension</param>
    /// <returns><c>true</c> if the theme was set successfully, otherwise <c>false</c></returns>
    private static bool SetThemeInternal(string theme)
    {
        try
        {
            if (Application.Current == null)
            {
                return false;
            }

            // Replace theme dictionary, which is the first entry in App.xaml
            Application.Current.Resources.MergedDictionaries[0].Source =
                UriUtils.BuildPackUri($"ResourceDictionaries/Themes/{theme}.xaml");

            return true;
        }
        catch (IOException)
        {
            return false;
        }
    }
}
