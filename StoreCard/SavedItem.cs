using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StoreCard
{
    public abstract class SavedItem
    {
        public string Name { get; protected set; }

        public string Base64Icon { get; protected set; }

        [JsonIgnore]
        public abstract ImageSource BitmapIcon { get; }

        protected SavedItem(string name, string base64Icon)
        {
            Name = name;
            Base64Icon = base64Icon;
        }

        public abstract void Open();
    }
}
