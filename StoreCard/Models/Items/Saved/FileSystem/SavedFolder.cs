#region

using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StoreCard.Static;
using StoreCard.Utils;

#endregion

namespace StoreCard.Models.Items.Saved.FileSystem;

/// <summary>
/// Represents a saved folder.
/// </summary>
public class SavedFolder : SavedFileSystemItem
{
    public SavedFolder(
        string id,
        string name,
        string? base64Icon,
        string itemPath,
        string executablePath,
        long lastOpened) : base(id,
        name,
        base64Icon,
        itemPath,
        executablePath,
        lastOpened)
    {
    }

    public override BitmapSource PrefixIcon => Icons.FolderIcon;

    public override ItemCategory Category => ItemCategory.Folder;

    public override SpecificItemCategory SpecificCategory => SpecificItemCategory.Folder;

    public override string SecondaryText => ItemCategory.Folder.ToString();

    public override bool Exists() => Directory.Exists(ItemPath);

    protected override ImageSource GetSystemIcon() => IconUtils.GetFolderIconByPath(ItemPath);
}
