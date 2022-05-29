#region

using Newtonsoft.Json;

#endregion

namespace StoreCard.GameLibraries.Itch;

public sealed class ButlerResponse<TResult>
{
    [JsonProperty("id")] public int Id;

    [JsonProperty("jsonrpc")] public string JsonRpc;

    [JsonProperty("result")] public TResult Result;

    public ButlerResponse(string jsonRpc, int id, TResult result)
    {
        JsonRpc = jsonRpc;
        Id = id;
        Result = result;
    }
}
