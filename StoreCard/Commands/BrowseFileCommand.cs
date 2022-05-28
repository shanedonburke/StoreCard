using Microsoft.Win32;
using StoreCard.Static;

namespace StoreCard.Commands;

public class BrowseFileCommand : IStoreCardCommand<string?>
{
    public string? Execute()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "All Files (*.*)|*.*",
            InitialDirectory = FolderPaths.UserProfile,
            Title = "Select File"
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }
}
