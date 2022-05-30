#region

using Newtonsoft.Json;

#endregion

namespace StoreCard.GameLibraries.Itch;

/// <summary>
/// An itch game as represented by Butler. This class is incomplete, because
/// we only need the game title.
/// </summary>
public sealed class ButlerGame
{
    [JsonProperty("title")] public readonly string Title;

    public ButlerGame(string title) => Title = title;
}
