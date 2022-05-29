#region

using Newtonsoft.Json;

#endregion

namespace StoreCard.GameLibraries.Itch;

public sealed class ButlerInstallInfo
{
    [JsonProperty("installFolder")] public readonly string InstallFolder;

    public ButlerInstallInfo(string installFolder) => InstallFolder = installFolder;
}
