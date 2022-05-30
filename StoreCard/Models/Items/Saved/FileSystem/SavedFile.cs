#region

using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StoreCard.Static;
using StoreCard.Utils;

#endregion

namespace StoreCard.Models.Items.Saved.FileSystem;

/// <summary>
/// Represents a saved file.
/// </summary>
public sealed class SavedFile : SavedFileSystemItem
{
    public SavedFile(
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

    public override BitmapSource PrefixIcon => Icons.FileIcon;

    public override ItemCategory Category => ItemCategory.File;

    public override SpecificItemCategory SpecificCategory => SpecificItemCategory.File;

    public override string SecondaryText => ItemCategory.File.ToString();

    public override bool Exists() => File.Exists(ItemPath);

    protected override ImageSource? GetSystemIcon() => IconUtils.GetFileIconByPath(ItemPath);
}
