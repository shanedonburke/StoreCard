using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace StoreCard
{
    public class ShowMainWindowCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            MainWindow? mainWindow = Application.Current.Windows
                .Cast<Window>()
                .Where(w => w is MainWindow)
                .FirstOrDefault() as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.Close();
            }
            var newWindow = new MainWindow();
            newWindow.Show();
            newWindow.Activate();
        }
    }
}
