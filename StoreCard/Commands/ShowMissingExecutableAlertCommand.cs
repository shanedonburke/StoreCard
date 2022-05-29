#region

using StoreCard.Models.Items.Saved.FileSystem;
using StoreCard.Windows;

#endregion

namespace StoreCard.Commands;

public sealed class ShowMissingExecutableAlertCommand : IStoreCardCommand<bool>
{
    private const string WindowTitle = "Missing Executable";
    private const string Explanation = "could not be found";

    private readonly SavedFileSystemItem _item;

    public ShowMissingExecutableAlertCommand(SavedFileSystemItem item) => _item = item;

    public bool Execute()
    {
        new InvalidExecutableWindow(_item, WindowTitle, Explanation).Show();
        return true;
    }
}
