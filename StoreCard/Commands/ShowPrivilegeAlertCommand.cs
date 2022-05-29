#region

using StoreCard.Models.Items.Saved.FileSystem;
using StoreCard.Windows;

#endregion

namespace StoreCard.Commands;

public sealed class ShowPrivilegeAlertCommand : IStoreCardCommand<bool>
{
    private const string WindowTitle = "Administrative Privileges Required";
    private const string Explanation = "requires administrative privileges";

    private readonly SavedFileSystemItem _item;

    public ShowPrivilegeAlertCommand(SavedFileSystemItem item) => _item = item;

    public bool Execute()
    {
        new InvalidExecutableWindow(_item, WindowTitle, Explanation).Show();
        return true;
    }
}
