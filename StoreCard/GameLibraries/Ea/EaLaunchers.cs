using Microsoft.Win32;
using StoreCard.Utils;

namespace StoreCard.GameLibraries.Ea;

internal class EaLaunchers
{
    public class Launcher
    {
        public readonly string DisplayName;
        public readonly string ProcessName;
        public readonly string? LauncherPath;

        internal Launcher(string displayName, string processName, string? launcherPath)
        {
            DisplayName = displayName;
            ProcessName = processName;
            LauncherPath = launcherPath;
        }

        public bool IsInstalled => LauncherPath != null;

        public bool IsRunning => ProcessUtils.IsProcessWithNameRunning(ProcessName);
    }

    public static Launcher Desktop = new(
        "EA Desktop",
        "EADesktop",
        Registry.GetValue(
            RegUtils.BuildRegistryPath(
                RegUtils.Keys.HkeyLocalMachine,
                RegUtils.Paths.Software32,
                "Electronic Arts",
                "EA Desktop"),
            "LauncherAppPath",
            null) as string);

    public static Launcher Origin = new(
        "Origin",
        "Origin",
        Registry.GetValue(
            RegUtils.BuildRegistryPath(
                RegUtils.Keys.HkeyLocalMachine,
                RegUtils.Paths.Software32,
                "Origin"
            ),
            "OriginPath",
            null) as string);

    public static Launcher? GetLauncher()
    {
        if (Desktop.IsInstalled && Desktop.IsRunning)
        {
            return Desktop;
        }

        if (Origin.IsInstalled && Origin.IsRunning)
        {
            return Origin;
        }

        if (Desktop.IsInstalled)
        {
            return Desktop;
        }

        if (Origin.IsInstalled)
        {
            return Origin;
        }

        return null;
    }
}
