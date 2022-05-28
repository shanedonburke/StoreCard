namespace StoreCard.Utils;

public static class RegUtils
{
    public static class Keys
    {
        public static readonly string HkeyLocalMachine = "HKEY_LOCAL_MACHINE";
        public static readonly string Software = "SOFTWARE";
        public static readonly string Wow6432Node = "Wow6432Node";
    }

    public static class Paths
    {
        public static readonly string Software64 = BuildRegistryPath(Keys.Software);
        public static readonly string Software32 = BuildRegistryPath(Keys.Software, Keys.Wow6432Node);
        public static readonly string CurrentVersion64 = BuildRegistryPath(Software64, "Microsoft", "Windows", "CurrentVersion");
        public static readonly string CurrentVersion32 = BuildRegistryPath(Software32, "Microsoft", "Windows", "CurrentVersion");
        public static readonly string Uninstall32 = BuildRegistryPath(CurrentVersion32, "Uninstall");
        public static readonly string StartupFolder64 = BuildRegistryPath(CurrentVersion64, "Explorer", "StartupApproved", "StartupFolder");
    }

    public static string BuildRegistryPath(params string[] keys)
    {
        return string.Join(@"\", keys);
    }
}
