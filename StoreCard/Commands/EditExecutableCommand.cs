using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreCard.Models.Items.Saved;
using StoreCard.Windows;

namespace StoreCard.Commands;

internal class EditExecutableCommand : IStoreCardCommand
{
    private readonly SavedExecutable _executable;

    public EditExecutableCommand(SavedExecutable executable)
    {
        _executable = executable;
    }

    public void Execute()
    {
        new EditExecutableWindow(_executable).Show();
    }
}