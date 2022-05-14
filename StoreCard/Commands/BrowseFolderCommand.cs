using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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