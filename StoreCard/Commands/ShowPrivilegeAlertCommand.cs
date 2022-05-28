using StoreCard.Models.Items.Saved.FileSystem;
using StoreCard.Windows;

namespace StoreCard.Commands;

public class ShowPrivilegeAlertCommand : IStoreCardCommand<bool>
{
    private readonly SavedFileSystemItem _item;

    public ShowPrivilegeAlertCommand(SavedFileSystemItem item)
    {
        _item = item;
    }

    public bool Execute()
    {
        new InvalidExecutableWindow(_item, "Administrative Privileges Required", "requires administrative privileges").Show();
        return true;
    }
}
