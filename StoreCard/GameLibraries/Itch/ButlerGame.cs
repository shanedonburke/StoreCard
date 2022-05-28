// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Newtonsoft.Json;

namespace StoreCard.GameLibraries.Itch;

internal class ButlerGame
{
    [JsonProperty("title")]
    public readonly string Title;

    public ButlerGame(string title) => Title = title;
}
