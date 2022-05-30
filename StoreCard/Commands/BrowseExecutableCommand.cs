#region

using System;
using Microsoft.Win32;

#endregion

namespace StoreCard.Commands;

/// <summary>
/// Opens a file browser dialog that allows the user to select a .exe file.
/// </summary>
public sealed class BrowseExecutableCommand : IStoreCardCommand<string?>
{
    /// <summary>
    /// Opens the dialog and waits for it to close.
    /// </summary>
    /// <returns>The selected file path, or <c>null</c> if the dialog was closed without a selection</returns>
    public string? Execute()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Executables|*.exe",
            // This always returns "Program Files" (not "x86"), unlike `Environment.SpecialFolder.ProgramFiles`
            InitialDirectory = Environment.ExpandEnvironmentVariables("%ProgramW6432%"),
            Title = "Select Executable"
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }
}
