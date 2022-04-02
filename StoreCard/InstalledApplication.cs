using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StoreCard
{
    public class InstalledApplication : IComparable<InstalledApplication>
    {
        public InstalledApplication(string name, string appUserModelId, BitmapSource icon)
        {
            Name = name;
            AppUserModelId = appUserModelId;
            Icon = icon;
        }

        public string Name { get; private set; }
        public string AppUserModelId { get; private set; }
        public BitmapSource Icon { get; private set; }

        public int CompareTo(InstalledApplication? other)
        {
            return Name.CompareTo(other?.Name);
        }
    }
}
