using System.Linq;
using System.Windows;
using StoreCard.Utils;
using StoreCard.Windows;

namespace StoreCard.Commands;

public class ShowSearchCommand : IStoreCardCommand<bool>
{
    public bool Execute()
    {
        var windows = Application.Current.Windows
            .Cast<Window>()
            .Where(w => w is not TaskbarIconWindow && w.GetType().Name != "AdornerWindow")
            .ToList();

        // If any windows are open, activate them instead of opening a new main window
        if (windows.Any())
        {
            foreach (Window window in windows)
            {
                window.BringToFront();
            }

            return true;
        }

        MainWindow newWindow = new();
        newWindow.Show();
        newWindow.Activate();

        return true;
    }
}
