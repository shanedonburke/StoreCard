#region

using StoreCard.Windows;

#endregion

namespace StoreCard.Commands;

/// <summary>
/// Shows the tutorial window.
/// </summary>
public sealed class ShowTutorialCommand : IStoreCardCommand<bool>
{
    /// <summary>
    /// Shows the tutorial window.
    /// </summary>
    /// <returns>True</returns>
    public bool Execute()
    {
        new TutorialWindow().Show();
        return true;
    }
}
