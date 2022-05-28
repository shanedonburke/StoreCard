using System;
using System.IO;
using System.Windows;

namespace StoreCard.Utils;

public class ThemeUtils
{
    public static void SetTheme(string theme)
    {
        if (SetThemepublic(theme))
        {
            return;
        }

        if (!SetThemepublic("Lake (Dark)"))
        {
            SetThemepublic(ThemeFinder.FindThemes()[0]);
        }
    }

    private static bool SetThemepublic(string theme)
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
