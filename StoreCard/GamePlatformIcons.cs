using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace StoreCard
{
    internal class GamePlatformIcons
    {
        public static readonly BitmapSource SteamIcon;

        static GamePlatformIcons()
        {
            SteamIcon = new BitmapImage(
                new Uri("pack://application:,,,/Icons/steam_icon.png"));
            SteamIcon.Freeze();
        }
    }
}
