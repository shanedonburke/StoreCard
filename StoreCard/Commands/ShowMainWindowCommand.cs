using System.Linq;
using System.Windows;
using StoreCard.Utils;
using StoreCard.Windows;

namespace StoreCard.Commands;

public class ShowMainWindowCommand : IStoreCardCommand<bool>
{
    public bool Execute()
    {
        WindowCollection windows = Application.Current.Windows;

        // If dialog window(s) are open, activate them instead of opening the main window
        if (windows.Count > 2)
        {
            // The first two windows are the system tray window and the invisible main one
            for (int i = 2; i < windows.Count; i++)
            {
                windows[i]?.BringToFront();
            }

            return true;
        }

        MainWindow? mainWindow = Application.Current.Windows
            .Cast<Window>()
            .FirstOrDefault(w => w is MainWindow) as MainWindow;

        mainWindow?.Close();

        MainWindow newWindow = new();
        newWindow.Show();
        newWindow.Activate();

        return true;
    }
}
