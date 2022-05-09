using System.IO;

namespace StoreCard.Models.Items.Saved;

internal class SavedFile : SavedFileSystemItem
{
    public SavedFile(string id, string name, string? base64Icon, string path, string executablePath)
        : base(id, name, base64Icon, path, executablePath)
    {
    }

    public override ItemCategory Category => ItemCategory.File;

    public override bool Exists()
    {
        return File.Exists(Path);
    }
}