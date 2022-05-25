using System.Windows.Media.Imaging;
using StoreCard.Utils;

namespace StoreCard.Static;

internal static class Icons
{
    public static readonly BitmapSource AppIcon;

    public static readonly BitmapSource GameIcon;

    public static readonly BitmapSource FileIcon;

    public static readonly BitmapSource FolderIcon;

    public static readonly BitmapSource LinkIcon;

    static Icons()
    {
        AppIcon = ImageUtils.BitmapToBitmapImage(Properties.Resources.AppIcon);
        GameIcon = ImageUtils.BitmapToBitmapImage(Properties.Resources.GameIcon);
        FileIcon = ImageUtils.BitmapToBitmapImage(Properties.Resources.FileIcon);
        FolderIcon = ImageUtils.BitmapToBitmapImage(Properties.Resources.FolderIcon);
        LinkIcon = ImageUtils.BitmapToBitmapImage(Properties.Resources.LinkIcon);
    }
}
