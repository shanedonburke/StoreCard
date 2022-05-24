using System.Windows.Media.Imaging;
using StoreCard.Utils;

namespace StoreCard.Static;

internal static class Icons
{
    public static readonly BitmapSource AppIcon;

    public static readonly BitmapSource GameIcon;

    public static readonly BitmapSource FileIcon;

    public static readonly BitmapSource LinkIcon;

    static Icons()
    {
        AppIcon = Images.BitmapToBitmapImage(Properties.Resources.AppIcon);
        GameIcon = Images.BitmapToBitmapImage(Properties.Resources.GameIcon);
        FileIcon = Images.BitmapToBitmapImage(Properties.Resources.FileIcon);
        LinkIcon = Images.BitmapToBitmapImage(Properties.Resources.LinkIcon);
    }
}
