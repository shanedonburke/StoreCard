// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using StoreCard.Models.Items.Saved.FileSystem;
using StoreCard.Windows;

namespace StoreCard.Commands;

public class ShowMissingExecutableAlertCommand : IStoreCardCommand<bool>
{
    private readonly SavedFileSystemItem _item;

    public ShowMissingExecutableAlertCommand(SavedFileSystemItem item)
    {
        _item = item;
    }

    public bool Execute()
    {
        new InvalidExecutableWindow(_item, "Missing Executable", "could not be found").Show();
        return true;
    }
}
