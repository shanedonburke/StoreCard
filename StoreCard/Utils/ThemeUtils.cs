using System;
using System.IO;
using System.Windows;

namespace StoreCard.Utils;

internal class ThemeUtils
{
    public static void SetTheme(string theme)
    {
        if (SetThemeInternal(theme))
        {
            return;
        }

        if (!SetThemeInternal("Lake (Dark)"))
        {
            SetThemeInternal(ThemeFinder.FindThemes()[0]);
        }
    }

    private static bool SetThemeInternal(string theme)
    {
        try
        {
            if (Application.Current == null)
            {
                return false;
            }

            Application.Current.Resources.MergedDictionaries[0].Source =
                new Uri($"pack://application:,,,/ResourceDictionaries/Themes/{theme}.xaml");

            return true;
        }
        catch (IOException)
        {
            return false;
        }
    }
}
