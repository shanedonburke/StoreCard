// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreCard.Models.Items.Saved.FileSystem;
using StoreCard.Windows;

namespace StoreCard.Commands;

internal class ShowPrivilegeAlertCommand : IStoreCardCommand<bool>
{
    private readonly SavedFileSystemItem _item;

    public ShowPrivilegeAlertCommand(SavedFileSystemItem item)
    {
        _item = item;
    }

    public bool Execute()
    {
        new InvalidExecutableWindow(_item, "Administrative Privileges Required", "requires administrative privileges").Show();
        return true;
    }
}
