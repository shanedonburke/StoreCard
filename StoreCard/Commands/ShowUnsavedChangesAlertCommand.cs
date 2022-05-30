// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#region

using StoreCard.Windows;

#endregion

namespace StoreCard.Commands;

/// <summary>
/// Shows an alert window to indicate that the user is trying to close a window
/// with unsaved changes.
/// </summary>
public sealed class ShowUnsavedChangesAlertCommand : IStoreCardCommand<UnsavedChangesWindow.Result>
{
    /// <summary>
    /// Shows the window as a modal dialog.
    /// </summary>
    /// <returns>A result indicating which button was pressed.</returns>
    public UnsavedChangesWindow.Result Execute()
    {
        var window = new UnsavedChangesWindow();
        window.ShowDialog();
        return window.DialogResult;
    }
}
