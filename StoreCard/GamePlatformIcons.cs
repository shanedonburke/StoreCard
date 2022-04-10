using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
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
