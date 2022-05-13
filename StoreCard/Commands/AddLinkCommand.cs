using StoreCard.Windows;

namespace StoreCard.Commands;

internal class AddLinkCommand : IStoreCardCommand<bool>
{
    public bool Execute()
    {
        new AddLinkWindow().Show();
        return true;
    }
}