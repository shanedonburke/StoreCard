#region

using System;
using Ookii.Dialogs.Wpf;

#endregion

namespace StoreCard.Commands;

public class BrowseFolderCommand : IStoreCardCommand<string?>
{
    public string? Execute()
    {
        var dialog = new VistaFolderBrowserDialog {RootFolder = Environment.SpecialFolder.UserProfile};

        return dialog.ShowDialog() == true ? dialog.SelectedPath : null;
    }
}
