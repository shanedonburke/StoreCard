using Newtonsoft.Json;

namespace StoreCard.GameLibraries.Itch;

public sealed class ButlerResponse<TResult>
{
    [JsonProperty("jsonrpc")]
    public string JsonRpc;

    [JsonProperty("id")]
    public int Id;

    [JsonProperty("result")]
    public TResult Result;

    public ButlerResponse(string jsonRpc, int id, TResult result)
    {
        JsonRpc = jsonRpc;
        Id = id;
        Result = result;
    }
}
