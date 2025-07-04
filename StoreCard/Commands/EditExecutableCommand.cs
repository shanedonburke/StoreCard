﻿#region

using StoreCard.Models.Items.Saved;
using StoreCard.Utils;
using StoreCard.Windows;

#endregion

namespace StoreCard.Commands;

/// <summary>
/// Opens a window for editing an executable-type item.
/// </summary>
public sealed class EditExecutableCommand : IStoreCardCommand<bool>
{
    /// <summary>
    /// The executable to be edited.
    /// </summary>
    private readonly SavedExecutable _executable;

    public EditExecutableCommand(SavedExecutable executable) => _executable = executable;

    /// <summary>
    /// Opens the window without waiting for it to close.
    /// </summary>
    /// <returns><c>true</c></returns>
    public bool Execute()
    {
        new EditExecutableWindow(_executable) { ShowActivated = true }.ShowForeground();
        return true;
    }
}
