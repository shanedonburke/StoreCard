using System.Windows.Media.Imaging;

namespace StoreCard
{
    internal class GamePlatformIcons
    {
        public static readonly BitmapSource SteamIcon;

        static GamePlatformIcons()
        {
            SteamIcon = ImageUtils.BitmapToBitmapImage(Properties.Resources.SteamIcon);
        }
    }
}
