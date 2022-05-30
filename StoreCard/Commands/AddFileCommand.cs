#region

using StoreCard.Windows;

#endregion

namespace StoreCard.Commands;

/// <summary>
/// Opens the window to add a new file item.
/// </summary>
public sealed class AddFileCommand : IStoreCardCommand<bool>
{
    /// <summary>
    /// Opens the window without waiting for it to close.
    /// </summary>
    /// <returns><c>true</c></returns>
    public bool Execute()
    {
        new AddFileWindow().Show();
        return true;
    }
}
