using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using StoreCard.Windows;

namespace StoreCard.Commands;

public class ShowMainWindowCommand : ICommand, IStoreCardCommand
{
    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute() {
        Execute(null);
    }

    public void Execute(object? parameter)
    {
        var mainWindow = Application.Current.Windows
            .Cast<Window>()
            .FirstOrDefault(w => w is MainWindow) as MainWindow;
        mainWindow?.Close();
        var newWindow = new MainWindow();
        newWindow.Show();
        newWindow.Activate();
    }
}