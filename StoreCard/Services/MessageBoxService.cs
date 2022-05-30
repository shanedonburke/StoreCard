#region

using System.Linq;
using System.Windows;
using StoreCard.Windows;

#endregion

namespace StoreCard.Services;

/// <summary>
/// A service to handle the display of message boxes.
/// </summary>
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

    /// <summary>
    /// Show a message box.
    /// </summary>
    /// <param name="messageBoxText">The text to display</param>
    /// <param name="caption">The window title/caption</param>
    /// <param name="button">The button(s) to show</param>
    /// <param name="icon">The icon to show in the window body</param>
    public void ShowMessageBox(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
    {
        // Get taskbar icon window if we don't have it
        _taskbarIconWindow ??= Application.Current.Windows.OfType<TaskbarIconWindow>().FirstOrDefault();
        // If the window that shows the message box closes, so does the box.
        // So, show it from a window that won't close.
        _taskbarIconWindow?.Dispatcher.BeginInvoke(() => MessageBox.Show(messageBoxText, caption, button, icon));
    }
}
