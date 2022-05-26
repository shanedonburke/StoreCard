using StoreCard.Models.Items.Saved;
using StoreCard.Models.Items.Saved.FileSystem;
using StoreCard.Windows;

namespace StoreCard.Commands;

internal class EditFileCommand : IStoreCardCommand<bool>
{
    private readonly SavedFileSystemItem _item;

    public EditFileCommand(SavedFileSystemItem item)
    {
        _item = item;
    }

    public bool Execute()
    {
        new EditFileWindow(_item).Show();
        return true;
    }
}
