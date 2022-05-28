// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Newtonsoft.Json;

namespace StoreCard.GameLibraries.Itch;

internal class ButlerResponse<TResult>
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
