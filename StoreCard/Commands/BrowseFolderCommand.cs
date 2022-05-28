using System;
using Ookii.Dialogs.Wpf;

namespace StoreCard.Commands;

internal class BrowseFolderCommand : IStoreCardCommand<string?>
{
    public string? Execute()
    {
        var dialog = new VistaFolderBrowserDialog
        {
            RootFolder = Environment.SpecialFolder.UserProfile
        };

        return dialog.ShowDialog() == true ? dialog.SelectedPath : null;
    }
}
