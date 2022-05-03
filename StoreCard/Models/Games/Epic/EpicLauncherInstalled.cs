using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreCard.Models.Games.Epic;

public class EpicLauncherInstalled
{
    public class InstalledApp
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