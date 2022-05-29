#region

using Microsoft.Win32;
using StoreCard.Static;

#endregion

namespace StoreCard.Commands;

/// <summary>
/// Opens a file browser dialog that allows the user to select a file of any type.
/// </summary>
public sealed class BrowseFileCommand : IStoreCardCommand<string?>
{
    /// <summary>
    /// Opens the dialog and waits for it to close.
    /// </summary>
    /// <returns>The selected file path, or null if the dialog was closed without a selection</returns>
    public string? Execute()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "All Files (*.*)|*.*", InitialDirectory = FolderPaths.UserProfile, Title = "Select File"
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }
}
