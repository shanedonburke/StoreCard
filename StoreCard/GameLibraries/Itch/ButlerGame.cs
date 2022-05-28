using Newtonsoft.Json;

namespace StoreCard.GameLibraries.Itch;

internal sealed class ButlerGame
{
    [JsonProperty("title")]
    public readonly string Title;

    public ButlerGame(string title) => Title = title;
}
