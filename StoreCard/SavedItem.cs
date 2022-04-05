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
    public enum ItemCategory : uint
    {
        None,
        App,
        Folder,
        File,
        Link
    }

    public abstract class SavedItem
    {
        public string Name { get; protected set; }

        public string? Base64Icon { get; protected set; }

        public abstract ItemCategory Category { get; }

        [JsonIgnore]
        public ImageSource? BitmapIcon => Base64Icon != null ? ImageUtils.Base64ToImage(Base64Icon) : null;

        protected SavedItem(string name, string? base64Icon)
        {
            Name = name;
            Base64Icon = base64Icon;
        }

        public abstract void Open();
    }
}
