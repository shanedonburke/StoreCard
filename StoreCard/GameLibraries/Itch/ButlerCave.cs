using Newtonsoft.Json;

namespace StoreCard.GameLibraries.Itch;

internal sealed class ButlerCave
{
    [JsonProperty("id")]
    public string Id;

    [JsonProperty("game")]
    public ButlerGame Game;

    [JsonProperty("installInfo")]
    public ButlerInstallInfo InstallInfo;

    public ButlerCave(string id, ButlerGame game, ButlerInstallInfo installInfo)
    {
        Id = id;
        Game = game;
        InstallInfo = installInfo;
    }
}
