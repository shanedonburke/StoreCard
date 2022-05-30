#region

using StoreCard.Windows;

#endregion

namespace StoreCard.Commands;

/// <summary>
/// Shows the settings window.
/// </summary>
public sealed class ShowSettingsCommand : IStoreCardCommand<bool>
{
    /// <summary>
    /// Shows the settings window.
    /// </summary>
    /// <returns>True</returns>
    public bool Execute()
    {
        var window = new SettingsWindow();
        window.Show();
        window.Activate();
        return true;
    }
}
