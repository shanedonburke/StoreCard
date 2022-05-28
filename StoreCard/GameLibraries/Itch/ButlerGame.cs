using Newtonsoft.Json;

namespace StoreCard.GameLibraries.Itch;

internal class ButlerGame
{
    [JsonProperty("title")]
    public readonly string Title;

    public ButlerGame(string title) => Title = title;
}
