#region

using StoreCard.Models.Items.Saved.FileSystem;
using StoreCard.Windows;

#endregion

namespace StoreCard.Commands;

public sealed class ShowMissingExecutableAlertCommand : IStoreCardCommand<bool>
{
    private readonly SavedFileSystemItem _item;

    public ShowMissingExecutableAlertCommand(SavedFileSystemItem item) => _item = item;

    public bool Execute()
    {
        const string windowTitle = "Missing Executable";
        const string explanation = "could not be found";

        new InvalidExecutableWindow(_item, windowTitle, explanation).Show();
        return true;
    }
}
