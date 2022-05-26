using System.Collections.Generic;
using StoreCard.Models.Items.Installed;
using StoreCard.Models.Items.Installed.Games;

namespace StoreCard.GameLibraries;

internal abstract class GameLibrary
{
    public abstract IEnumerable<InstalledGame> GetInstalledGames();
}
