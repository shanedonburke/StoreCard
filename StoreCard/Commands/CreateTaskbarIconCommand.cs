#region

using System.Linq;
using System.Windows;
using StoreCard.Windows;

#endregion

namespace StoreCard.Commands;

/// <summary>
/// Opens the invisible window for the taskbar icon if it isn't already open.
/// </summary>
public sealed class CreateTaskbarIconCommand : IStoreCardCommand<bool>
{
    /// <summary>
    /// Whether the taskbar icon window is already open.
    /// </summary>
    private static bool DoesTaskbarIconExist => Application.Current.Windows
        .Cast<Window>()
        .Any(w => w is TaskbarIconWindow);

    /// <summary>
    /// Opens the window if it isn't already open.
    /// </summary>
    /// <returns><c>true</c> if a new window was opened, <c>false</c> if the window was already open</returns>
    public bool Execute()
    {
        if (DoesTaskbarIconExist)
        {
            return false;
        }

        new TaskbarIconWindow().Show();
        return true;
    }
}
