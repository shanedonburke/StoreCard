#region

using StoreCard.Utils;
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
    /// <returns><c>true</c></returns>
    public bool Execute()
    {
        new SettingsWindow() { ShowActivated = true }.ShowForeground();
        return true;
    }
}
