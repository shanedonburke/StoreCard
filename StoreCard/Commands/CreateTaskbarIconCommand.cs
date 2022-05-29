#region

using System.Linq;
using System.Windows;
using StoreCard.Windows;

#endregion

namespace StoreCard.Commands;

public class CreateTaskbarIconCommand : IStoreCardCommand<bool>
{
    public bool Execute()
    {
        if (Application.Current.Windows.Cast<Window>().Any(w => w is TaskbarIconWindow))
        {
            return false;
        }

        new TaskbarIconWindow().Show();
        return true;
    }
}
