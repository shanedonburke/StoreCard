using System.Collections.Generic;

namespace StoreCard.Models.Games.Epic;

public class InstalledItemManifiest
{
    public int FormatVersion;
    public bool bIsIncompleteInstall;
    public string LaunchCommand;
    public string LaunchExecutable;
    public string ManifestLocation;
    public bool bIsApplication;
    public bool bIsExecutable;
    public bool bIsManaged;
    public bool bNeedsValidation;
    public bool bRequiresAuth;
    public bool bAllowMultipleInstances;
    public bool bCanRunOffline;
    public bool bAllowUriCmdArgs;
    public List<string> BaseURLs;
    public string BuildLabel;
    public List<string> AppCategories;
    public string DisplayName;
    public string InstallationGuid;
    public string InstallLocation;
    public string InstallSessionId;
    public string HostInstallationGuid;
    public List<string> PrereqIds;
    public string StagingLocation;
    public string TechnicalType;
    public string VaultThumbnailUrl;
    public string VaultTitleText;
    public int InstallSize;
    public string MainWindowProcessName;
    public string MandatoryAppFolderName;
    public string OwnershipToken;
    public string CatalogNamespace;
    public string CatalogItemId;
    public string AppName;
    public string AppVersionString;
    public string MainGameCatalogNamespace;
    public string MainGameCatalogItemId;
    public string MainGameAppName;
}