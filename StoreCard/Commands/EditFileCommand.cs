using System;
using System.Diagnostics;
using System.Windows.Input;
using StoreCard.Models.Items.Saved;
using StoreCard.Windows;

namespace StoreCard.Commands;

internal class EditFileCommand : IStoreCardCommand
{
    private readonly SavedFileSystemItem _item;

    public EditFileCommand(SavedFileSystemItem item)
    {
        _item = item;
    }

    public void Execute()
    {
        new EditFileWindow(_item).Show();
    }
}