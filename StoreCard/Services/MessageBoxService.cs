#region

using System.Linq;
using System.Windows;
using StoreCard.Windows;

#endregion

namespace StoreCard.Services;

public sealed class MessageBoxService
{
    public static readonly MessageBoxService Instance = new();

    private TaskbarIconWindow? _taskbarIconWindow;

    static MessageBoxService()
    {
    }

    private MessageBoxService()
    {
    }

    public void ShowMessageBox(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
    {
        _taskbarIconWindow ??= Application.Current.Windows.OfType<TaskbarIconWindow>().FirstOrDefault();
        _taskbarIconWindow?.Dispatcher.BeginInvoke(() => MessageBox.Show(messageBoxText, caption, button, icon));
    }
}
