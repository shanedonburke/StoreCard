using StoreCard.Windows;

namespace StoreCard.Commands;

internal class AddApplicationCommand : IStoreCardCommand<bool>
{
    public bool Execute()
    {
        new AddApplicationWindow().Show();
        return true;
    }
}