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

        public string Name { get; }
        public string AppUserModelId { get; }
        public string? ExecutablePath { get; }
        public BitmapSource BitmapIcon { get; }

        public int CompareTo(InstalledApplication? other)
        {
            return Compare(Name, other?.Name, StringComparison.Ordinal);
        }
    }
}
