#region

using StoreCard.Windows;

#endregion

namespace StoreCard.Commands;

public sealed class AddFileCommand : IStoreCardCommand<bool>
{
    public bool Execute()
    {
        new AddFileWindow().Show();
        return true;
    }
}
