namespace StoreCard.GameLibraries.Epic;

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
