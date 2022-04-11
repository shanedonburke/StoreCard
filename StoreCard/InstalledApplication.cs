using System;
using System.Windows.Media.Imaging;
using static System.String;

namespace StoreCard
{
    public class InstalledApplication : IComparable<InstalledApplication>
    {
        public InstalledApplication(string name, string appUserModelId, string? executablePath, BitmapSource icon)
        {
            Name = name;
            AppUserModelId = appUserModelId;
            ExecutablePath = executablePath;
            BitmapIcon = icon;
        }

        public string Name { get; private set; }
        public string AppUserModelId { get; private set; }
        public string? ExecutablePath { get; private set; }
        public BitmapSource BitmapIcon { get; private set; }

        public int CompareTo(InstalledApplication? other)
        {
            return Compare(Name, other?.Name, StringComparison.Ordinal);
        }
    }
}
