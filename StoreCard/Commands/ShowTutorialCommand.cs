﻿#region

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
    /// <returns><c>true</c></returns>
    public bool Execute()
    {
        new TutorialWindow().Show();
        return true;
    }
}
