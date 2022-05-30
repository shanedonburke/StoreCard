using System.Linq;
using System.Windows;
using StoreCard.Utils;
using StoreCard.Windows;

namespace StoreCard.Commands;

/// <summary>
/// Shows the main window, i.e., the searchable item list.
/// </summary>
public sealed class ShowSearchCommand : IStoreCardCommand<bool>
{
    /// <summary>
    /// Shows the main window if no other windows are open. If another window
    /// is open, it is brought to the front.
    /// </summary>
    /// <returns><c>true</c></returns>
    public bool Execute()
    {
        // Windows besides the taskbar icon window and any adorners added by Visual Studio
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
