#region

using System.Collections.Generic;
using StoreCard.Properties;

#endregion

namespace StoreCard.GameLibraries.BattleNet;

public static class BattleNetGameIds
{
    private static readonly Dictionary<string, string> s_gameIds = new();

    public static string? GetGameId(string uid)
    {
        if (s_gameIds.Count == 0)
        {
            PopulateDictionary();
        }

        s_gameIds.TryGetValue(uid, out string? gameId);
        return gameId;
    }

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
