using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace StoreCard.GameLibraries.Itch;

internal class ButlerInstallInfo
{
    [JsonProperty("installFolder")]
    public readonly string InstallFolder;

    public ButlerInstallInfo(string installFolder) => InstallFolder = installFolder;
}
