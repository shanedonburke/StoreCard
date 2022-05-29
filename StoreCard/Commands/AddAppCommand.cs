﻿#region

using StoreCard.Windows;

#endregion

namespace StoreCard.Commands;

/// <summary>
/// Opens the window for adding a new app/executable item.
/// </summary>
public sealed class AddAppCommand : IStoreCardCommand<bool>
{
    /// <summary>
    /// Opens the window without waiting for it to close.
    /// </summary>
    /// <returns>True</returns>
    public bool Execute()
    {
        new AddAppWindow().Show();
        return true;
    }
}
