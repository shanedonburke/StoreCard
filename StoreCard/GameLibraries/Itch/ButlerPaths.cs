#region

using System.IO;
using System.Linq;
using StoreCard.Static;

#endregion

namespace StoreCard.GameLibraries.Itch;

/// <summary>
/// This class is used to group various file paths that are needed by itch launcher classes.
/// </summary>
public static class ButlerPaths
{
    /// <summary>
    /// Folder where the itch launcher stores various things.
    /// </summary>
    private static readonly string s_itchDataFolder =
        Path.Combine(FolderPaths.ApplicationData, "itch");

    /// <summary>
    /// Path to the Butler daemon executable.
    /// </summary>
    public static readonly string? ButlerExecutable = Directory.EnumerateFiles(
        Path.Combine(s_itchDataFolder, @"broth\butler\versions"),
        "butler.exe",
        SearchOption.AllDirectories).ToList().FirstOrDefault();

    /// <summary>
    /// Path to the Butler DB file.
    /// </summary>
    public static readonly string ButlerDatabase = Path.Combine(s_itchDataFolder, @"db\butler.db");

    /// <summary>
    /// Path that must be passed to Butler when launching a game.
    /// </summary>
    public static readonly string ButlerPrereqsFolder = Path.Combine(s_itchDataFolder, @"prereqs");
}
