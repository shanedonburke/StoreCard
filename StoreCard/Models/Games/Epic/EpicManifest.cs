﻿namespace StoreCard.Models.Games.Epic;

public class EpicManifest
{
    public int FormatVersion;
    public bool bIsIncompleteInstall;
    public string LaunchExecutable;
    public string DisplayName;
    public string InstallLocation;
    public string AppName;

    public EpicManifest(int formatVersion, bool bIsIncompleteInstall, string launchExecutable, string displayName,
        string installLocation, string appName)
    {
        FormatVersion = formatVersion;
        this.bIsIncompleteInstall = bIsIncompleteInstall;
        LaunchExecutable = launchExecutable;
        DisplayName = displayName;
        InstallLocation = installLocation;
        AppName = appName;
    }
}