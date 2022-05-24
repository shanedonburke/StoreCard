using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StoreCard.Static;
using StoreCard.Utils;

namespace StoreCard.Models.Items.Saved;

internal class SavedFile : SavedFileSystemItem
{
    public SavedFile(string id, string name, string? base64Icon, string path, string executablePath, long lastOpened)
        : base(id, name, base64Icon, path, executablePath, lastOpened)
    {
    }

    public override BitmapSource? PrefixIcon => Icons.FileIcon;

    public override ItemCategory Category => ItemCategory.File;

    public override SpecificItemCategory SpecificCategory => SpecificItemCategory.File;

    public override string SecondaryText => ItemCategory.File.ToString();

    public override bool Exists()
    {
        return File.Exists(Path);
    }

    protected override ImageSource? GetSystemIcon()
    {
        return IconUtils.GetFileIconByPath(Path);
    }
}
