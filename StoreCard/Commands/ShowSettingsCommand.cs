using StoreCard.Windows;

namespace StoreCard.Commands;

public class ShowSettingsCommand : IStoreCardCommand<bool>
{
    public bool Execute()
    {
        var window = new SettingsWindow();
        window.Show();
        window.Activate();
        return true;
    }
}
