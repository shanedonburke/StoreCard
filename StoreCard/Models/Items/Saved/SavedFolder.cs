using System.IO;

namespace StoreCard.Models.Items.Saved;

internal class SavedFolder : SavedFileSystemItem
{
    public SavedFolder(string id, string name, string? base64Icon, string path, string executablePath, long lastOpened)
        : base(id, name, base64Icon, path, executablePath, lastOpened)
    {
    }

    public override ItemCategory Category => ItemCategory.Folder;

    public override SpecificItemCategory SpecificCategory => SpecificItemCategory.Folder;

    public override bool Exists()
    {
        return Directory.Exists(Path);
    }
}