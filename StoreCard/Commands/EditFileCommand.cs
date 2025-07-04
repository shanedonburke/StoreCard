﻿#region

using StoreCard.Models.Items.Saved.FileSystem;
using StoreCard.Utils;
using StoreCard.Windows;

#endregion

namespace StoreCard.Commands;

/// <summary>
/// Opens a window for editing a file or folder item.
/// </summary>
public sealed class EditFileCommand : IStoreCardCommand<bool>
{
    /// <summary>
    /// The item to be edited.
    /// </summary>
    private readonly SavedFileSystemItem _item;

    public EditFileCommand(SavedFileSystemItem item) => _item = item;

    /// <summary>
    /// Opens the window without waiting for it to close.
    /// </summary>
    /// <returns><c>true</c></returns>
    public bool Execute()
    {
        new EditFileWindow(_item) { ShowActivated = true }.ShowForeground();
        return true;
    }
}
