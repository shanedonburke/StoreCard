#region

using Newtonsoft.Json;

#endregion

namespace StoreCard.GameLibraries.Itch;

/// <summary>
/// Represents the JSON object returned as a response to Butler daemon requests.
/// </summary>
/// <typeparam name="TResult">Type of the <see cref="Result"/> property, which is request type-specific.</typeparam>
public sealed class ButlerResponse<TResult>
{
    /// <summary>
    /// The arbitrary ID we passed in with the request.
    /// </summary>
    [JsonProperty("id")] public int Id;

    /// <summary>
    /// The request-specific result.
    /// </summary>
    [JsonProperty("result")] public TResult Result;

    [JsonProperty("jsonrpc")] public string JsonRpc;

    public ButlerResponse(string jsonRpc, int id, TResult result)
    {
        JsonRpc = jsonRpc;
        Id = id;
        Result = result;
    }
}
