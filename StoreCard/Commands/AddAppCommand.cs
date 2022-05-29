#region

using StoreCard.Windows;

#endregion

namespace StoreCard.Commands;

public sealed class AddAppCommand : IStoreCardCommand<bool>
{
    public bool Execute()
    {
        new AddAppWindow().Show();
        return true;
    }
}
