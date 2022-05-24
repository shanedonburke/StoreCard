// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
