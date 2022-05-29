#region

using StoreCard.Models.Items.Saved.FileSystem;
using StoreCard.Windows;

#endregion

namespace StoreCard.Commands;

public class EditFileCommand : IStoreCardCommand<bool>
{
    private readonly SavedFileSystemItem _item;

    public EditFileCommand(SavedFileSystemItem item) => _item = item;

    public bool Execute()
    {
        new EditFileWindow(_item).Show();
        return true;
    }
}
