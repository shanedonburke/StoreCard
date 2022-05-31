namespace StoreCard.Utils;

/// <summary>
/// Utilities for working with the registry.
/// </summary>
public static class RegUtils
{
    /// <summary>
    /// Build a registry key path by joining the given segments.
    /// </summary>
    /// <param name="segments">Path segments</param>
    /// <returns>A full registry path, assuming the given segments form one</returns>
    public static string BuildRegistryPath(params string[] segments) => string.Join(@"\", segments);

    /// <summary>
    /// Common registry key segments
    /// </summary>
    public static class Keys
    {
        public static readonly string HkeyLocalMachine = "HKEY_LOCAL_MACHINE";
        public static readonly string Software = "SOFTWARE";
        public static readonly string Wow6432Node = "Wow6432Node";
    }

    /// <summary>
    /// Common registry key paths.
    /// </summary>
    public static class Paths
    {
        public static readonly string Software64 = BuildRegistryPath(Keys.Software);
        public static readonly string Software32 = BuildRegistryPath(Keys.Software, Keys.Wow6432Node);

        public static readonly string CurrentVersion64 =
            BuildRegistryPath(Software64, "Microsoft", "Windows", "CurrentVersion");

        public static readonly string StartupFolder64 =
            BuildRegistryPath(CurrentVersion64, "Explorer", "StartupApproved", "StartupFolder");
    }
}
