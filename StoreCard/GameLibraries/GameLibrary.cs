#region

using System.Collections.Generic;
using StoreCard.Models.Items.Installed.Games;

#endregion

namespace StoreCard.GameLibraries;

/// <summary>
/// Represents the library of installed games for a game launcher.
/// </summary>
public abstract class GameLibrary
{
    /// <summary>
    /// Get the games that are currently installed for this launcher.
    /// </summary>
    /// <returns>An enumerable of installed games</returns>
    public abstract IEnumerable<InstalledGame> GetInstalledGames();
}
