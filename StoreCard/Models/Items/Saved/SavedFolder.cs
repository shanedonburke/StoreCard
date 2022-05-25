using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StoreCard.Static;
using StoreCard.Utils;

namespace StoreCard.Models.Items.Saved;

internal class SavedFolder : SavedFileSystemItem
{
    public SavedFolder(string id, string name, string? base64Icon, string path, string executablePath, long lastOpened)
        : base(id, name, base64Icon, path, executablePath, lastOpened)
    {
    }

    public override BitmapSource? PrefixIcon => Icons.FolderIcon;

    public override ItemCategory Category => ItemCategory.Folder;

    public override SpecificItemCategory SpecificCategory => SpecificItemCategory.Folder;

    public override string SecondaryText => ItemCategory.Folder.ToString();

    public override bool Exists()
    {
        return Directory.Exists(Path);
    }

    protected override ImageSource GetSystemIcon()
    {
        return IconUtils.GetFolderIconByPath(Path);
    }
}
