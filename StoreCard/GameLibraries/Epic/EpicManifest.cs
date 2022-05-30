namespace StoreCard.GameLibraries.Epic;

/// <summary>
/// Represents a JSON file used by the Epic Games launcher
/// that includes details about a specific game.
/// </summary>
public class EpicManifest
{
    public string AppName;
    public bool BIsIncompleteInstall;
    public string DisplayName;
    public int FormatVersion;
    public string InstallLocation;
    public string LaunchExecutable;

    public EpicManifest(
        int formatVersion,
        bool bIsIncompleteInstall,
        string launchExecutable,
        string displayName,
        string installLocation,
        string appName)
    {
        FormatVersion = formatVersion;
        BIsIncompleteInstall = bIsIncompleteInstall;
        LaunchExecutable = launchExecutable;
        DisplayName = displayName;
        InstallLocation = installLocation;
        AppName = appName;
    }
}
