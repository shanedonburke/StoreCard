using StoreCard.Windows;

namespace StoreCard.Commands;

internal class OpenSettingsCommand : IStoreCardCommand<bool>
{
    public bool Execute()
    {
        new SettingsWindow().Show();
        return true;
    }
}