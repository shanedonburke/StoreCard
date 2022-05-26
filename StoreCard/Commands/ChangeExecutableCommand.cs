using System;
using StoreCard.Models.Items.Saved;
using StoreCard.Models.Items.Saved.FileSystem;
using StoreCard.Windows;

namespace StoreCard.Commands;

internal class ChangeExecutableCommand : IStoreCardCommand<bool>
{
    private readonly SavedFileSystemItem _item;

    public ChangeExecutableCommand(SavedFileSystemItem item)
    {
        _item = item;
    }

    public bool Execute()
    {
        return new ChangeExecutableWindow(_item).ShowDialog() ?? false;
    }
}
