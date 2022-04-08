using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace StoreCard
{
    public class InstalledSteamGame : InstalledGame
    {
        private static readonly BitmapSource _platformIcon;

        static InstalledSteamGame()
        {
            _platformIcon = new BitmapImage(
                new Uri("pack://application:,,,/Icons/steam_icon.png"));
            _platformIcon.Freeze();
        }

        public InstalledSteamGame(
            string name,
            string appId,
            BitmapSource bitmapIcon) : base(name, bitmapIcon)
        {
            AppId = appId;
        }

        public string AppId { get; private set; }

        public override BitmapSource PlatformIcon => _platformIcon;

        public override SavedItem SavedItem => new SavedSteamGame(this);
    }
}
