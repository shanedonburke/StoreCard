using System.Linq;
using System.Windows;
using StoreCard.Windows;

namespace StoreCard.Commands;

public class ShowMainWindowCommand : IStoreCardCommand<bool>
{
    public bool Execute()
    {
        var mainWindow = Application.Current.Windows
            .Cast<Window>()
            .FirstOrDefault(w => w is MainWindow) as MainWindow;
        mainWindow?.Close();
        var newWindow = new MainWindow();
        newWindow.Show();
        newWindow.Activate();
        return true;
    }
}