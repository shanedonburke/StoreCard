#region

using StoreCard.Windows;

#endregion

namespace StoreCard.Commands;

public sealed class AddApplicationCommand : IStoreCardCommand<bool>
{
    public bool Execute()
    {
        new AddAppWindow().Show();
        return true;
    }
}
