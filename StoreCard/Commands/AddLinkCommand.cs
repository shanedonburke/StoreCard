#region

using StoreCard.Windows;

#endregion

namespace StoreCard.Commands;

public sealed class AddLinkCommand : IStoreCardCommand<bool>
{
    public bool Execute()
    {
        new AddLinkWindow().Show();
        return true;
    }
}
