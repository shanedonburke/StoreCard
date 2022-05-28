// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Newtonsoft.Json;

namespace StoreCard.GameLibraries.Itch;

internal class ButlerCave
{
    [JsonProperty("id")]
    public string Id;

    [JsonProperty("game")]
    public ButlerGame Game;

    [JsonProperty("installInfo")]
    public ButlerInstallInfo InstallInfo;

    public ButlerCave(string id, ButlerGame game, ButlerInstallInfo installInfo)
    {
        Id = id;
        Game = game;
        InstallInfo = installInfo;
    }
}
