using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreCard.GameLibraries.BattleNet;

internal static class BattleNetGameIds
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
        foreach (string line in Properties.Resources.BattleNetGameIds.Split("\n"))
        {
            string[] parts = line.Split("=");
            if (parts.Length == 2)
            {
                s_gameIds[parts[0]] = parts[1].Trim();
            }
        }
    }
}
