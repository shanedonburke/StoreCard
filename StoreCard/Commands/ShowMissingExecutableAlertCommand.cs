#region

using StoreCard.Models.Items.Saved.FileSystem;
using StoreCard.Windows;

#endregion

namespace StoreCard.Commands;

/// <summary>
/// Shows an alert window indicating that an executable (used to open a file system item)
/// could not be found.
/// </summary>
public sealed class ShowMissingExecutableAlertCommand : IStoreCardCommand<bool>
{
    /// <summary>
    /// The window title.
    /// </summary>
    private const string WindowTitle = "Missing Executable";

    /// <summary>
    /// A snippet explaining the alert.
    /// </summary>
    private const string Explanation = "could not be found";

    /// <summary>
    /// The item we are trying to open.
    /// </summary>
    private readonly SavedFileSystemItem _item;

    /// <summary>
    /// Creates the command.
    /// </summary>
    /// <param name="item">The item we are trying to open</param>
    public ShowMissingExecutableAlertCommand(SavedFileSystemItem item) => _item = item;

    /// <summary>
    /// Opens the window without waiting for it to close.
    /// </summary>
    /// <returns>True</returns>
    public bool Execute()
    {
        new InvalidExecutableWindow(_item, WindowTitle, Explanation).Show();
        return true;
    }
}
