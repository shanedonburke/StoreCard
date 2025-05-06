#region

using StoreCard.Utils;
using StoreCard.Windows;

#endregion

namespace StoreCard.Commands;

/// <summary>
/// Opens the window to add a new link item.
/// </summary>
public sealed class AddLinkCommand : IStoreCardCommand<bool>
{
    /// <summary>
    /// Opens the window without waiting for it to close.
    /// </summary>
    /// <returns><c>true</c></returns>
    public bool Execute()
    {
        new AddLinkWindow() { ShowActivated = true }.ShowForeground();
        return true;
    }
}
