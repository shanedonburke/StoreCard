using StoreCard.Windows;

namespace StoreCard.Commands;

public class ShowTutorialCommand : IStoreCardCommand<bool>
{
    public bool Execute()
    {
        new TutorialWindow().Show();
        return true;
    }
}
