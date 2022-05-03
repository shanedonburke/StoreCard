using System.Windows.Media.Imaging;
using StoreCard.Utils;

namespace StoreCard.Static;

internal class Icons
{
    public static readonly BitmapSource LinkIcon;

    public static readonly BitmapSource GameIcon;

    static Icons()
    {
        LinkIcon = ImageUtils.BitmapToBitmapImage(Properties.Resources.LinkIcon);
        GameIcon = ImageUtils.BitmapToBitmapImage(Properties.Resources.GameIcon);
    }
}