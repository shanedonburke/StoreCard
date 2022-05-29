#region

using Microsoft.Win32;
using StoreCard.Static;

#endregion

namespace StoreCard.Commands;

public sealed class BrowseFileCommand : IStoreCardCommand<string?>
{
    public string? Execute()
    {
        const string dialogFilter = "All Files (*.*)|*.*";
        const string dialogTitle = "Select File";

        var dialog = new OpenFileDialog
        {
            Filter = dialogFilter, InitialDirectory = FolderPaths.UserProfile, Title = dialogTitle
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }
}
