#region

using Microsoft.Win32;
using StoreCard.Utils;

#endregion

namespace StoreCard.GameLibraries.Ea;

/// <summary>
/// Includes logic for the EA game launchers.
/// There is significant overlap between the functionality of
/// the Origin launcher and the new EA Desktop launcher. Either one,
/// both, or neither may be installed. For opening an EA game,
/// either one will do.
/// </summary>
public sealed class EaLaunchers
{
    /// <summary>
    /// The EA Desktop launcher.
    /// </summary>
    private static readonly Launcher s_desktop = new(
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

    /// <summary>
    /// The Origin launcher.
    /// </summary>
    private static readonly Launcher s_origin = new(
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

    /// <summary>
    /// Gets an installed launcher, or null if there isn't one.
    /// The EA Desktop launcher is preferred, as this one will likely
    /// be more widely supported in the future.
    /// </summary>
    /// <returns>An installed launcher</returns>
    public static Launcher? GetLauncher()
    {
        if (s_desktop.IsInstalled && s_desktop.IsRunning)
        {
            return s_desktop;
        }

        if (s_origin.IsInstalled && s_origin.IsRunning)
        {
            return s_origin;
        }

        if (s_desktop.IsInstalled)
        {
            return s_desktop;
        }

        return s_origin.IsInstalled ? s_origin : null;
    }

    /// <summary>
    /// Represents an EA game launcher.
    /// </summary>
    public sealed class Launcher
    {
        /// <summary>
        /// An arbitrary name to use for the launcher.
        /// </summary>
        public readonly string DisplayName;

        /// <summary>
        /// The path to the launcher executable.
        /// </summary>
        public readonly string? LauncherPath;

        /// <summary>
        /// The name of the launcher's system process when it's running
        /// </summary>
        public readonly string ProcessName;

        /// <summary>
        /// Creates the launcher
        /// </summary>
        /// <param name="displayName">An arbitrary name to use for the launcher</param>
        /// <param name="processName">The path to the launcher executable</param>
        /// <param name="launcherPath">The name of the launcher's system process when it's running</param>
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
