using StoreCard.Windows;

namespace StoreCard.Commands;

public class AddApplicationCommand : IStoreCardCommand<bool>
{
    public bool Execute()
    {
        new AddAppWindow().Show();
        return true;
    }
}