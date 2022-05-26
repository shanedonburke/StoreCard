﻿using System;
using StoreCard.Models.Items.Saved;
using StoreCard.Models.Items.Saved.FileSystem;
using StoreCard.Windows;

namespace StoreCard.Commands;

internal class ChangeExecutableCommand : IStoreCardCommand<bool>
{
    private readonly SavedFileSystemItem _item;

    private readonly bool _shouldShowAsDialog;

    public ChangeExecutableCommand(SavedFileSystemItem item, bool shouldShowAsDialog)
    {
        _item = item;
        _shouldShowAsDialog = shouldShowAsDialog;
    }

    public bool Execute()
    {
        if (_shouldShowAsDialog)
        {
            return new ChangeExecutableWindow(_item).ShowDialog() ?? false;
        }

        new ChangeExecutableWindow(_item).Show();
        return true;
    }
}
