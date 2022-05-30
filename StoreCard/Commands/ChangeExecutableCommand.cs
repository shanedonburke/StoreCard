#region

using StoreCard.Models.Items.Saved.FileSystem;
using StoreCard.Windows;

#endregion

namespace StoreCard.Commands;

/// <summary>
/// Opens a window that prompts the user to select a new executable to be used when opening
/// the given item.
/// </summary>
public sealed class ChangeExecutableCommand : IStoreCardCommand<bool>
{
    /// <summary>
    /// The item for which a new executable will be selected.
    /// </summary>
    private readonly SavedFileSystemItem _item;

    /// <summary>
    /// Whether the window should be shown as a modal dialog instead of a normal window.
    /// </summary>
    private readonly bool _shouldShowAsDialog;

    /// <summary>
    /// Creates the command.
    /// </summary>
    /// <param name="item">The item for which a new executable will be selected</param>
    /// <param name="shouldShowAsDialog">
    /// Whether the window should be shown as a modal dialog instead of a normal window
    /// </param>
    public ChangeExecutableCommand(SavedFileSystemItem item, bool shouldShowAsDialog)
    {
        _item = item;
        _shouldShowAsDialog = shouldShowAsDialog;
    }

    /// <summary>
    /// If the window is a modal dialog, opens the window and waits for it to close.
    /// Otherwise, the window is opened without waiting for it to close.
    /// </summary>
    /// <returns><c>false</c> if the window was a modal dialog and no new executable was chosen, <c>true</c> otherwise</returns>
    public bool Execute()
    {
        if (_shouldShowAsDialog)
        {
            return new ChangeExecutableWindow(_item).ShowDialog() ?? false;
        }

        new ChangeExecutableWindow(_item).Show();
        return true;
    }
}
