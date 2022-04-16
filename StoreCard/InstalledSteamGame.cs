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
        public InstalledSteamGame(
            string name,
            string appId,
            BitmapSource bitmapIcon) : base(name, bitmapIcon)
        {
            AppId = appId;
        }

        public string AppId { get; private set; }

        public override BitmapSource PlatformIcon => Icons.SteamIcon;

        public override SavedItem SavedItem => new SavedSteamGame(this);
    }
}
