#region

using StoreCard.Models.Items.Saved;
using StoreCard.Windows;

#endregion

namespace StoreCard.Commands;

/// <summary>
/// Opens a window for editing a link item.
/// </summary>
public sealed class EditLinkCommand : IStoreCardCommand<bool>
{
    /// <summary>
    /// The link to be edited.
    /// </summary>
    private readonly SavedLink _link;

    /// <summary>
    /// Creates the command.
    /// </summary>
    /// <param name="link">The link to be edited</param>
    public EditLinkCommand(SavedLink link) => _link = link;

    /// <summary>
    /// Opens the window without waiting for it to close.
    /// </summary>
    /// <returns>True</returns>
    public bool Execute()
    {
        new EditLinkWindow(_link).Show();
        return true;
    }
}
