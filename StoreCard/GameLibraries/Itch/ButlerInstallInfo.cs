using Newtonsoft.Json;

namespace StoreCard.GameLibraries.Itch;

internal class ButlerInstallInfo
{
    [JsonProperty("installFolder")]
    public readonly string InstallFolder;

    public ButlerInstallInfo(string installFolder) => InstallFolder = installFolder;
}
