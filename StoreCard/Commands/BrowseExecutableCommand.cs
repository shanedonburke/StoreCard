#region

using System;
using Microsoft.Win32;

#endregion

namespace StoreCard.Commands;

internal class BrowseExecutableCommand : IStoreCardCommand<string?>
{
    public string? Execute()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Executables|*.exe",
            // This always returns "Program Files", unlike `Environment.SpecialFolder.ProgramFiles`
            InitialDirectory = Environment.ExpandEnvironmentVariables("%ProgramW6432%"),
            Title = "Select Executable"
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }
}
