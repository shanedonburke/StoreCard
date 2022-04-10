using System;
using System.Windows.Media.Imaging;
using static System.String;

namespace StoreCard
{
    public abstract class InstalledGame : IComparable<InstalledGame>
    {
        protected InstalledGame(string name, BitmapSource bitmapIcon)
        {
            Name = name;
            BitmapIcon = bitmapIcon;
        }

        public string Name { get; }
        public BitmapSource BitmapIcon { get; }

        public abstract BitmapSource PlatformIcon { get; }
        public abstract SavedItem SavedItem { get; }

        public int CompareTo(InstalledGame? other)
        {
            return Compare(Name, other?.Name, StringComparison.Ordinal);
        }
    }
}
