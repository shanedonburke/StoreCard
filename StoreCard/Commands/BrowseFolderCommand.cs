#region

using System.Windows.Forms;
using StoreCard.Static;

#endregion

namespace StoreCard.Commands;

/// <summary>
/// Opens a folder browser dialog that allows the user to select a local folder.
/// </summary>
public sealed class BrowseFolderCommand : IStoreCardCommand<string?>
{
    /// <summary>
    /// Opens the dialog and waits for it to close.
    /// </summary>
    /// <returns>The selected folder path, or <c>null</c> if the dialog was closed without a selection</returns>
    public string? Execute()
    {
        var dialog = new FolderBrowserDialog {InitialDirectory = FolderPaths.UserProfile};
        return dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPath : null;
    }
}
