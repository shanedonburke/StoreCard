using System.IO;

namespace StoreCard.Models.Items.Saved;

internal class SavedFolder : SavedFileSystemItem
{
    public SavedFolder(string id, string name, string? base64Icon, string path, string executablePath)
        : base(id, name, base64Icon, path, executablePath)
    {
    }

    public override ItemCategory Category => ItemCategory.Folder;

    public override bool Exists() {
        return Directory.Exists(Path);
    }
}