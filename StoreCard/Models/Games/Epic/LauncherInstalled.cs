using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreCard.Models.Games.Epic;

public class LauncherInstalled
{
    public class InstalledApp
    {
        public string InstallLocation;

        public string NamespaceId;

        public string ItemId;

        public string ArtifactId;

        public string AppVersion;

        public string AppName;
    }

    public List<InstalledApp> InstallationList;
}