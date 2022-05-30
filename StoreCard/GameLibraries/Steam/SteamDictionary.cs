#region

using System.Collections.Generic;
using System.Text.RegularExpressions;

#endregion

namespace StoreCard.GameLibraries.Steam;

/// <summary>
/// Steam stores data in the VDF (Valve Data File) format, which is similar to JSON.
/// The top-level element of a VDF file is an "object" that contains properties, some of
/// which may be objects themselves.
///
/// This class handles the parsing and representation of these objects.
/// As such, an instance of this class may represent a whole file or a single child object.
/// </summary>
public sealed class SteamDictionary
{
    /// <summary>
    /// The opening bracket for a child object is preceded by its key on a separate line.
    /// This expression captures the key. Example:
    /// <code>
    /// "apps"
    /// {
    ///   ...
    /// }
    /// </code>
    /// </summary>
    private static readonly Regex s_keyRegex = new(@"^\s*""(?<key>[^<>""/|?*]+)""\s*$");

    /// <summary>
    /// Captures a key-value pair. Example:
    /// <code>"key"    "value"</code>
    /// </summary>
    private static readonly Regex s_pairRegex = new(@"^\s*""(?<key>[^<>""/|?*]+)""\s+""(?<value>[^<>""/|?*]+)""$");

    /// <summary>
    /// Captures a closing bracket (surrounded by whitespace), signifying the end of an object.
    /// </summary>
    private static readonly Regex s_closeChildRegex = new(@"^\s*}\s*$");

    private readonly Dictionary<string, SteamDictionary> _children = new();

    
    private readonly Dictionary<string, string> _pairs = new();

    /// <summary>
    /// Key-value pairs that belong to this object (excluding ones where the value is an object).
    /// </summary>
    public IReadOnlyDictionary<string, string> Pairs => _pairs;

    /// <summary>
    /// Child objects of this one.
    /// </summary>
    public IReadOnlyDictionary<string, SteamDictionary> Children => _children;

    public static SteamDictionary Parse(string dictionary)
    {
        if (dictionary.Trim() == string.Empty)
        {
            return new SteamDictionary();
        }

        // Represent the nesting of objects as a stack. The object we're currently parsing
        // is at the top of the stack. Pop an object off when we're done parsing it.
        Stack<SteamDictionary> stack = new();
        SteamDictionary root = new();
        stack.Push(root);

        string[] lines = dictionary.Split('\n');

        // Skip the root object's key and opening bracket, then parse each line
        for (int i = 2; i < lines.Length; i++)
        {
            if (lines[i].Trim() == string.Empty)
            {
                continue;
            }

            Match pairMatch = s_pairRegex.Match(lines[i]);

            // The line is a key-value pair; add it to the current object
            if (pairMatch.Success)
            {
                string key = pairMatch.Groups["key"].Value;
                string value = pairMatch.Groups["value"].Value;
                stack.Peek().AddPair(key, value);
                continue;
            }

            Match keyMatch = s_keyRegex.Match(lines[i]);

            // The line is the start of a child object; add it to the stack
            if (keyMatch.Success)
            {
                SteamDictionary child = new();
                stack.Peek().AddChild(keyMatch.Groups["key"].Value, child);
                stack.Push(child);
                continue;
            }

            // We've reached the end of an object
            if (s_closeChildRegex.IsMatch(lines[i]))
            {
                stack.Pop();
            }

            // Ignore the line if it's an opening bracket
        }

        return root;
    }

    /// <summary>
    /// Adds a key-value pair to the dictionary
    /// </summary>
    /// <param name="key">The key to use</param>
    /// <param name="value">The value to use</param>
    public void AddPair(string key, string value) => _pairs[key] = value;

    /// <summary>
    /// Adds a child object to the dictionary under the given key.
    /// </summary>
    /// <param name="key">The key to use</param>
    /// <param name="child">The value (a child object)</param>
    public void AddChild(string key, SteamDictionary child) => _children[key] = child;
}
