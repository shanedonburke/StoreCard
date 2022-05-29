#region

using Newtonsoft.Json;

#endregion

namespace StoreCard.GameLibraries.Itch;

public sealed class ButlerCave
{
    [JsonProperty("game")] public ButlerGame Game;

    [JsonProperty("id")] public string Id;

    [JsonProperty("installInfo")] public ButlerInstallInfo InstallInfo;

    public ButlerCave(string id, ButlerGame game, ButlerInstallInfo installInfo)
    {
        Id = id;
        Game = game;
        InstallInfo = installInfo;
    }
}
