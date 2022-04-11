using System;
using System.Diagnostics;

namespace StoreCard
{
    internal class SavedFile : SavedItem
    {
        public string Path { get; }

        public SavedFile(string name, string? base64Icon, string path) : base(name, base64Icon)
        {
            Path = path;
        }

        public override ItemCategory Category => ItemCategory.File;

        public override void Open()
        {
            SystemUtils.OpenInDefaultProgram(Path);
        }
    }
}
