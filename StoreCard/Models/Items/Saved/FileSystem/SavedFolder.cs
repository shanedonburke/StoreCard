#region

using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StoreCard.Static;
using StoreCard.Utils;

#endregion

namespace StoreCard.Models.Items.Saved.FileSystem;

public class SavedFolder : SavedFileSystemItem
{
    public SavedFolder(string id,
        string name,
        string? base64Icon,
        string itemItemPath,
        string executablePath,
        long lastOpened)
        : base(id, name, base64Icon, itemItemPath, executablePath, lastOpened)
    {
    }

    public override BitmapSource PrefixIcon => Icons.FolderIcon;

    public override ItemCategory Category => ItemCategory.Folder;

    public override SpecificItemCategory SpecificCategory => SpecificItemCategory.Folder;

    public override string SecondaryText => ItemCategory.Folder.ToString();

    public override bool Exists() => Directory.Exists(ItemPath);

    protected override ImageSource GetSystemIcon() => IconUtils.GetFolderIconByPath(ItemPath);
}
