using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace StoreCard
{
    public abstract class InstalledGame : IComparable<InstalledGame>
    {
        protected InstalledGame(string name, BitmapSource bitmapIcon)
        {
            Name = name;
            BitmapIcon = bitmapIcon;
        }

        public string Name { get; private set; }
        public BitmapSource BitmapIcon { get; private set; }

        public abstract SavedItem SavedItem { get; }

        public int CompareTo(InstalledGame? other)
        {
            return Name.CompareTo(other?.Name);
        }
    }
}
