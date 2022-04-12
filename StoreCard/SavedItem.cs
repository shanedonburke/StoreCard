using System;
using Newtonsoft.Json;
using System.Windows.Media;
using static System.String;

namespace StoreCard
{
    public enum ItemCategory : uint
    {
        None,
        App,
        Game,
        Folder,
        File,
        Link
    }

    public abstract class SavedItem : IComparable<SavedItem>
    {
        public string Name { get; protected set; }

        public string? Base64Icon { get; protected set; }

        public abstract ItemCategory Category { get; }

        [JsonIgnore]
        public ImageSource? BitmapIcon => Base64Icon != null ? ImageUtils.Base64ToImage(Base64Icon) : null;

        [JsonIgnore]
        public virtual ImageSource? PrefixIcon { get; } = null;

        protected SavedItem(string name, string? base64Icon)
        {
            Name = name;
            Base64Icon = base64Icon;
        }

        public abstract void Open();

        public int CompareTo(SavedItem? other)
        {
            return Compare(Name, other?.Name, StringComparison.Ordinal);
        }
    }
}
