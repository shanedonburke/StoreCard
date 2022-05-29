#region

using System.IO;
using System.Linq;
using StoreCard.Static;

#endregion

namespace StoreCard.GameLibraries.Itch;

public static class ButlerPaths
{
    private static readonly string s_itchDataFolder =
        Path.Combine(FolderPaths.ApplicationData, "itch");

    public static readonly string? ButlerExecutable = Directory.EnumerateFiles(
        Path.Combine(s_itchDataFolder, @"broth\butler\versions"),
        "butler.exe",
        SearchOption.AllDirectories).ToList().FirstOrDefault();

    public static readonly string ButlerDatabase = Path.Combine(s_itchDataFolder, @"db\butler.db");

    public static readonly string ButlerPrereqsFolder = Path.Combine(s_itchDataFolder, @"prereqs");
}
