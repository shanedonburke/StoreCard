#region

using System.Collections.Generic;

#endregion

namespace StoreCard.GameLibraries.Epic;

public sealed class EpicLauncherInstalled
{
    public List<InstalledApp> InstallationList;

    public EpicLauncherInstalled(List<InstalledApp> installationList) => InstallationList = installationList;

    public sealed class InstalledApp
    {
        public string AppName;

        public InstalledApp(string appName) => AppName = appName;
    }
}
