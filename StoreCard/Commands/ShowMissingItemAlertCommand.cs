#region

using System;
using StoreCard.Models.Items.Saved;
using StoreCard.Windows;

#endregion

namespace StoreCard.Commands;

/// <summary>
/// Opens an alert window indicating that the item we are trying to open does not exist
/// (applicable to file system and executable items).
/// </summary>
public sealed class ShowMissingItemAlertCommand : IStoreCardCommand<bool>
{
    /// <summary>
    /// The action invoked if the user chooses to edit the item.
    /// If this is null, there will be no option to edit the item.
    /// </summary>
    private readonly Action? _editAction;

    /// <summary>
    /// The item we are trying to open.
    /// </summary>
    private readonly SavedItem _item;

    /// <summary>
    /// Creates the command for a window with no option to edit the item.
    /// </summary>
    /// <param name="item">The item we are trying to open</param>
    public ShowMissingItemAlertCommand(SavedItem item) => _item = item;

    /// <summary>
    /// Creates the command for a window with an option to edit the item.
    /// </summary>
    /// <param name="item">The item we are trying to open</param>
    /// <param name="editAction">The action invoked when the user chooses to edit the item</param>
    public ShowMissingItemAlertCommand(SavedItem item, Action editAction) : this(item) => _editAction = editAction;

    /// <summary>
    /// Opens the window. If the "Edit" button is available, the window is shown as a modal dialog,
    /// and the command waits for the window to be closed. If there is no "Edit" button, the window is
    /// opened normally without waiting for it to close.
    /// </summary>
    /// <returns>True</returns>
    public bool Execute()
    {
        if (_editAction != null)
        {
            new MissingItemWindow(_item, _editAction).ShowDialog();
        }
        else
        {
            new MissingItemWindow(_item).ShowDialog();
        }

        return true;
    }
}
