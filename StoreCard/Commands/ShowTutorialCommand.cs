using StoreCard.Windows;

namespace StoreCard.Commands;

internal class ShowTutorialCommand : IStoreCardCommand<bool>
{
    public bool Execute()
    {
        new TutorialWindow().Show();
        return true;
    }
}
