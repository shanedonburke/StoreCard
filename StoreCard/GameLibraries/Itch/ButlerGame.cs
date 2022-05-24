// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace StoreCard.GameLibraries.Itch;

internal class ButlerGame
{
    [JsonProperty("title")]
    public readonly string Title;

    public ButlerGame(string title) => Title = title;
}
