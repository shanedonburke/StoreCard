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

        public string AppId { get; }

        public override BitmapSource PrefixIcon => Icons.SteamIcon;

        public override SavedItem SavedItem => new SavedSteamGame(this);
    }
}
