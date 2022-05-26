using StoreCard.Windows;

namespace StoreCard.Commands;

internal class AddApplicationCommand : IStoreCardCommand<bool>
{
    public bool Execute()
    {
        new AddAppWindow().Show();
        return true;
    }
}