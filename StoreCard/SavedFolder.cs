namespace StoreCard
{
    internal class SavedFolder : SavedFileSystemItem
    {
        public SavedFolder(string name, string? base64Icon, string path, string executablePath)
            : base(name, base64Icon, path, executablePath)
        {
        }

        public override ItemCategory Category => ItemCategory.Folder;
    }
}