#region

using Newtonsoft.Json;

#endregion

namespace StoreCard.GameLibraries.Itch;

public sealed class ButlerGame
{
    [JsonProperty("title")] public readonly string Title;

    public ButlerGame(string title) => Title = title;
}
