#region

using System;
using StoreCard.Models.Items.Saved;
using StoreCard.Windows;

#endregion

namespace StoreCard.Commands;

/// <summary>
/// Opens an alert window indicating that the item we are trying to open requires
/// administrative privileges
/// (applicable to file system and executable items).
/// </summary>
public sealed class ShowPrivilegeAlertCommand : IStoreCardCommand<bool>
{
    private const string WindowTitle = "Administrative Privileges Required";

    private const string Explanation = "requires administrative privileges";

    /// <summary>
    /// The item we are trying to open.
    /// </summary>
    private readonly SavedItem _item;

    /// <summary>
    /// Name of the executable that requires admin privileges.
    /// </summary>
    private readonly string _executableName;

    /// <summary>
    /// The action invoked when the "Edit" button is pressed.
    /// </summary>
    private readonly Action _editAction;

    /// <summary>
    /// Create the command.
    /// </summary>
    /// <param name="item">The item we are trying to open</param>
    /// <param name="executableName">Name of the executable that requires admin privileges</param>
    /// <param name="editAction">The action invoked when the "Edit" button is pressed</param>
    public ShowPrivilegeAlertCommand(SavedItem item, string executableName, Action editAction)
    {
        _item = item;
        _executableName = executableName;
        _editAction = editAction;
    }

    /// <summary>
    /// Opens the alert window.
    /// </summary>
    /// <returns>True</returns>
    public bool Execute()
    {
        new InvalidExecutableWindow(_item, _executableName, WindowTitle, Explanation, _editAction).Show();
        return true;
    }
}
