#region

using System.Collections.Generic;
using StoreCard.Models.Items.Installed.Games;

#endregion

namespace StoreCard.GameLibraries;

public abstract class GameLibrary
{
    public abstract IEnumerable<InstalledGame> GetInstalledGames();
}
