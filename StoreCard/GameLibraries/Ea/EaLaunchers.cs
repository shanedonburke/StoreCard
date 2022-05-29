#region

using Microsoft.Win32;
using StoreCard.Utils;

#endregion

namespace StoreCard.GameLibraries.Ea;

public sealed class EaLaunchers
{
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

        return Origin.IsInstalled ? Origin : null;
    }

    public sealed class Launcher
    {
        public readonly string DisplayName;
        public readonly string? LauncherPath;
        public readonly string ProcessName;

        public Launcher(string displayName, string processName, string? launcherPath)
        {
            DisplayName = displayName;
            ProcessName = processName;
            LauncherPath = launcherPath;
        }

        public bool IsInstalled => LauncherPath != null;

        public bool IsRunning => ProcessUtils.IsProcessWithNameRunning(ProcessName);
    }
}
