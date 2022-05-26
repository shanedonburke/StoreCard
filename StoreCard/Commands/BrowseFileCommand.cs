using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using StoreCard.Static;

namespace StoreCard.Commands;

internal class BrowseFileCommand : IStoreCardCommand<string?>
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
