#region

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
    /// <returns>True</returns>
    public bool Execute()
    {
        new AddLinkWindow().Show();
        return true;
    }
}
