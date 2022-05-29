#region

using StoreCard.Models.Items.Saved.FileSystem;
using StoreCard.Windows;

#endregion

namespace StoreCard.Commands;

public sealed class ShowPrivilegeAlertCommand : IStoreCardCommand<bool>
{
    private readonly SavedFileSystemItem _item;

    public ShowPrivilegeAlertCommand(SavedFileSystemItem item) => _item = item;

    public bool Execute()
    {
        const string windowTitle = "Administrative Privileges Required";
        const string explanation = "requires administrative privileges";

        new InvalidExecutableWindow(_item, windowTitle, explanation).Show();
        return true;
    }
}
