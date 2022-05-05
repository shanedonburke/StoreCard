using System.Collections.Generic;
using StoreCard.Models.Items.Installed;

namespace StoreCard.GameLibraries;

internal abstract class GameLibrary
{
    public abstract IEnumerable<InstalledGame> GetInstalledGames();
}