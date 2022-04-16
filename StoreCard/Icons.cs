using System.Windows.Media.Imaging;

namespace StoreCard
{
    internal class Icons
    {
        public static readonly BitmapSource SteamIcon;

        public static readonly BitmapSource LinkIcon;

        static Icons()
        {
            SteamIcon = ImageUtils.BitmapToBitmapImage(Properties.Resources.SteamIcon);
            LinkIcon = ImageUtils.BitmapToBitmapImage(Properties.Resources.LinkIcon);
        }
    }
}
