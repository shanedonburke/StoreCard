using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace StoreCard.GameLibraries.Steam;

public sealed class SteamDictionary
{
    private static readonly Regex s_keyRegex = new(@"^\s*""(?<key>[^<>""/|?*]+)""\s*$");

    private static readonly Regex s_pairRegex = new(@"^\s*""(?<key>[^<>""/|?*]+)""\s+""(?<value>[^<>""/|?*]+)""$");

    private static readonly Regex s_closeChildRegex = new(@"^\s*}\s*$");

    private readonly Dictionary<string, string> _pairs = new();

    private readonly Dictionary<string, SteamDictionary> _children = new();

    public IReadOnlyDictionary<string, string> Pairs => _pairs;

    public IReadOnlyDictionary<string, SteamDictionary> Children => _children;

    public static SteamDictionary Parse(string dictionary)
    {
        if (dictionary.Trim() == string.Empty)
        {
            return new SteamDictionary();
        }

        SteamDictionary root = new();
        Stack<SteamDictionary> stack = new();
        stack.Push(root);

        string[] lines = dictionary.Split('\n');

        for (int i = 2; i < lines.Length; i++)
        {
            if (lines[i].Trim() == string.Empty)
            {
                continue;
            }

            Match pairMatch = s_pairRegex.Match(lines[i]);

            if (pairMatch.Success)
            {
                string key = pairMatch.Groups["key"].Value;
                string value = pairMatch.Groups["value"].Value;
                stack.Peek().AddPair(key, value);
                continue;
            }

            Match keyMatch = s_keyRegex.Match(lines[i]);

            if (keyMatch.Success)
            {
                SteamDictionary child = new();
                stack.Peek().AddChild(keyMatch.Groups["key"].Value, child);
                stack.Push(child);
                continue;
            }

            if (s_closeChildRegex.IsMatch(lines[i]))
            {
                stack.Pop();
            }

            // Ignore the line if it's an opening bracket
        }

        return root;
    }

    public void AddPair(string key, string value)
    {
        _pairs[key] = value;
    }

    public void AddChild(string key, SteamDictionary child)
    {
        _children[key] = child;
    }
}
