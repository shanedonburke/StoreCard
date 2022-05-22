using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace StoreCard.Commands;

class BrowseExecutableCommand : IStoreCardCommand<string?>
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
