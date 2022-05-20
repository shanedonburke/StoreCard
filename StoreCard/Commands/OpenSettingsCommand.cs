using StoreCard.Windows;

namespace StoreCard.Commands;

internal class OpenSettingsCommand : IStoreCardCommand<bool>
{
    public bool Execute()
    {
        var window = new SettingsWindow();
        window.Show();
        window.Activate();
        return true;
    }
}
