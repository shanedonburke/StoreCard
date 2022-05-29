#region

using System.Linq;
using System.Windows;
using StoreCard.Windows;

#endregion

namespace StoreCard.Commands;

public sealed class CreateTaskbarIconCommand : IStoreCardCommand<bool>
{
    private static bool DoesTaskbarIconExist => Application.Current.Windows
        .Cast<Window>()
        .Any(w => w is TaskbarIconWindow);

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
