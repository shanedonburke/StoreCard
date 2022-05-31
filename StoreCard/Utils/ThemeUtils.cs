#region

using System.IO;
using System.Windows;

#endregion

namespace StoreCard.Utils;

public static class ThemeUtils
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
                UriUtils.BuildPackUri($"ResourceDictionaries/Themes/{theme}.xaml");

            return true;
        }
        catch (IOException)
        {
            return false;
        }
    }
}
