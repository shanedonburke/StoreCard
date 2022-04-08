using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreCard
{
    public class SavedSteamGame : SavedItem
    {
        public string AppId { get; private set; }

        public override ItemCategory Category => ItemCategory.Game;

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
            throw new NotImplementedException();
        }
    }
}
