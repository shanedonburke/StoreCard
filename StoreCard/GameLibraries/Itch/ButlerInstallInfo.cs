#region

using Newtonsoft.Json;

#endregion

namespace StoreCard.GameLibraries.Itch;

/// <summary>
/// The installation info of an itch game as represented by Butler.
/// This class is incomplete, because we only need the installation folder.
/// </summary>
public sealed class ButlerInstallInfo
{
    [JsonProperty("installFolder")] public readonly string InstallFolder;

    public ButlerInstallInfo(string installFolder) => InstallFolder = installFolder;
}
