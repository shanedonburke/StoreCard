﻿using System.Collections.Generic;

namespace StoreCard.GameLibraries.Epic;

public sealed class EpicLauncherInstalled
{
    public sealed class InstalledApp
    {
        public string AppName;

        public InstalledApp(string appName)
        {
            AppName = appName;
        }
    }

    public List<InstalledApp> InstallationList;

    public EpicLauncherInstalled(List<InstalledApp> installationList)
    {
        InstallationList = installationList;
    }
}
