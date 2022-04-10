using System.Diagnostics;

namespace StoreCard
{
    internal class SavedExecutable : SavedItem
    {
        public string Path { get; }

        public override ItemCategory Category => ItemCategory.App;

        public SavedExecutable(string name, string? base64Icon, string path) : base(name, base64Icon)
        {
            Path = path;
        }

        public override void Open()
        {
            Process.Start(Path);
        }
    }
}
