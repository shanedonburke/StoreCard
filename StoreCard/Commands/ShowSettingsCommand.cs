#region

using StoreCard.Windows;

#endregion

namespace StoreCard.Commands;

public sealed class ShowSettingsCommand : IStoreCardCommand<bool>
{
    public bool Execute()
    {
        var window = new SettingsWindow();
        window.Show();
        window.Activate();
        return true;
    }
}
