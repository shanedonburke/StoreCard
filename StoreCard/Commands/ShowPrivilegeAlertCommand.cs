#region

using System;
using StoreCard.Models.Items.Saved;
using StoreCard.Models.Items.Saved.FileSystem;
using StoreCard.Windows;

#endregion

namespace StoreCard.Commands;

/// <summary>
/// Opens an alert window indicating that the item we are trying to open requires
/// administrative privileges
/// (applicable to file system items).
/// </summary>
public sealed class ShowPrivilegeAlertCommand : IStoreCardCommand<bool>
{
    private const string WindowTitle = "Administrative Privileges Required";
    private const string Explanation = "requires administrative privileges";

    private readonly SavedItem _item;

    private readonly string _executableName;

    private readonly Action _editAction;

    public ShowPrivilegeAlertCommand(SavedItem item, string executableName, Action editAction)
    {
        _item = item;
        _executableName = executableName;
        _editAction = editAction;
    }

    public bool Execute()
    {
        new InvalidExecutableWindow(_item, _executableName, WindowTitle, Explanation, _editAction).Show();
        return true;
    }
}
