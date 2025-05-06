#region

using StoreCard.Models.Items.Saved.FileSystem;
using StoreCard.Utils;
using StoreCard.Windows;

#endregion

namespace StoreCard.Commands;

/// <summary>
/// Shows an alert window indicating that an executable (used to open a file system item)
/// could not be found.
/// </summary>
public sealed class ShowMissingExecutableAlertCommand : IStoreCardCommand<bool>
{
    private const string WindowTitle = "Missing Executable";

    private const string Explanation = "could not be found";

    /// <summary>
    /// The item we are trying to open.
    /// </summary>
    private readonly SavedFileSystemItem _item;

    public ShowMissingExecutableAlertCommand(SavedFileSystemItem item) => _item = item;

    /// <summary>
    /// Opens the window without waiting for it to close.
    /// </summary>
    /// <returns><c>true</c></returns>
    public bool Execute()
    {
        new InvalidExecutableWindow(
                _item,
                _item.ExecutableName,
                WindowTitle,
                Explanation,
                () => new ChangeExecutableCommand(_item, false).Execute())
        { ShowActivated = true }.ShowForeground();
        return true;
    }
}
