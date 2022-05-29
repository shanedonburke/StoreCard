#region

using System.Windows.Media.Imaging;
using StoreCard.Properties;
using StoreCard.Utils;

#endregion

namespace StoreCard.Static;

public static class Icons
{
    public static readonly BitmapSource AppIcon;

    public static readonly BitmapSource GameIcon;

    public static readonly BitmapSource FileIcon;

    public static readonly BitmapSource FolderIcon;

    public static readonly BitmapSource LinkIcon;

    static Icons()
    {
        AppIcon = ImageUtils.BitmapToBitmapImage(Resources.AppIcon);
        GameIcon = ImageUtils.BitmapToBitmapImage(Resources.GameIcon);
        FileIcon = ImageUtils.BitmapToBitmapImage(Resources.FileIcon);
        FolderIcon = ImageUtils.BitmapToBitmapImage(Resources.FolderIcon);
        LinkIcon = ImageUtils.BitmapToBitmapImage(Resources.LinkIcon);
    }
}
