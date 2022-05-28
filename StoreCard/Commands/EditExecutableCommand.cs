using StoreCard.Models.Items.Saved;
using StoreCard.Windows;

namespace StoreCard.Commands;

public class EditExecutableCommand : IStoreCardCommand<bool>
{
    private readonly SavedExecutable _executable;

    public EditExecutableCommand(SavedExecutable executable)
    {
        _executable = executable;
    }

    public bool Execute()
    {
        new EditExecutableWindow(_executable).Show();
        return true;
    }
}