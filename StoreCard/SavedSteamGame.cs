using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace StoreCard
{
    public class SavedSteamGame : SavedItem
    {
        public string AppId { get; private set; }

        public override ItemCategory Category => ItemCategory.Game;

        public override ImageSource? PrefixIcon => GamePlatformIcons.SteamIcon;

        [JsonConstructor]
        public SavedSteamGame(
            string name,
            string base64Icon,
            string appId) : base(name, base64Icon)
        {
            AppId = appId;
        }

        public SavedSteamGame(InstalledSteamGame game) : base(
            game.Name,
            ImageUtils.ImageToBase64(game.BitmapIcon)
        )
        {
            AppId = game.AppId;
        }

        public override void Open()
        {
            if (Paths.SteamInstallFolder == null)
            {
                MessageBox.Show("The Steam installation folder could not be found.");
                return;
            }
            string steamExecPath = Path.Combine(Paths.SteamInstallFolder, "steam.exe");
            Process.Start(steamExecPath, $"steam://rungameid/{AppId}");
        }
    }
}
