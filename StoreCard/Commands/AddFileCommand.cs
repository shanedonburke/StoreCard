using StoreCard.Windows;

namespace StoreCard.Commands;

internal class AddFileCommand : IStoreCardCommand<bool>
{
    public bool Execute()
    {
        new AddFileWindow().Show();
        return true;
    }
}