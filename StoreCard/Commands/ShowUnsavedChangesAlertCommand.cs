// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#region

using StoreCard.Windows;

#endregion

namespace StoreCard.Commands;

internal class ShowUnsavedChangesAlertCommand : IStoreCardCommand<UnsavedChangesWindow.Result>
{
    public UnsavedChangesWindow.Result Execute()
    {
        var window = new UnsavedChangesWindow();
        window.ShowDialog();
        return window.DialogResult;
    }
}
