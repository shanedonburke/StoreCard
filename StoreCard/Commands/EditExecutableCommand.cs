#region

using StoreCard.Models.Items.Saved;
using StoreCard.Windows;

#endregion

namespace StoreCard.Commands;

/// <summary>
/// Opens a window for editing an executable-type item.
/// </summary>
public sealed class EditExecutableCommand : IStoreCardCommand<bool>
{
    /// <summary>
    /// The executable to be edited.
    /// </summary>
    private readonly SavedExecutable _executable;

    /// <summary>
    /// Creates the command.
    /// </summary>
    /// <param name="executable">The executable to be edited</param>
    public EditExecutableCommand(SavedExecutable executable) => _executable = executable;

    /// <summary>
    /// Opens the window without waiting for it to close.
    /// </summary>
    /// <returns></returns>
    public bool Execute()
    {
        new EditExecutableWindow(_executable).Show();
        return true;
    }
}
