﻿#region

using StoreCard.Models.Items.Saved;
using StoreCard.Windows;

#endregion

namespace StoreCard.Commands;

public sealed class EditExecutableCommand : IStoreCardCommand<bool>
{
    private readonly SavedExecutable _executable;

    public EditExecutableCommand(SavedExecutable executable) => _executable = executable;

    public bool Execute()
    {
        new EditExecutableWindow(_executable).Show();
        return true;
    }
}
