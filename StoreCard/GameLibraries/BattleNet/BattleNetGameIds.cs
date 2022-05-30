#region

using System.Collections.Generic;
using StoreCard.Properties;

#endregion

namespace StoreCard.GameLibraries.BattleNet;

/// <summary>
/// Parses <c>BattleNetGameIds.txt</c> into a dictionary that maps Battle.net game UIDs
/// to the (slightly different) game IDs that are used internally by the launcher.
/// There is no good way to obtain these dynamically, so they are hard-coded in that file.
/// </summary>
public static class BattleNetGameIds
{
    /// <summary>
    /// Map of game UIDs to internal game IDs.
    /// </summary>
    private static readonly Dictionary<string, string> s_gameIds = new();

    /// <summary>
    /// Gets the internal game ID for the given game UID.
    /// </summary>
    /// <param name="uid">A game UID, probably from the registry</param>
    /// <returns>The game ID, or <c>null</c> if the ID isn't known</returns>
    public static string? GetGameId(string uid)
    {
        if (s_gameIds.Count == 0)
        {
            PopulateDictionary();
        }

        s_gameIds.TryGetValue(uid, out string? gameId);
        return gameId;
    }

    /// <summary>
    /// Populates the dictionary by parsing the text file. The format
    /// is one game on each line, <c>{uid}={internal ID}</c>.
    /// </summary>
    private static void PopulateDictionary()
    {
        foreach (string line in Resources.BattleNetGameIds.Split('\n'))
        {
            string[] parts = line.Split('=');

            if (parts.Length == 2)
            {
                s_gameIds[parts[0]] = parts[1].Trim();
            }
        }
    }
}
