// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using StoreCard.Models.Items.Installed;
using StoreCard.Utils;

namespace StoreCard.Models.Items.Saved;

internal class SavedEaGame : SavedGame
{
    public readonly string GameId;

    [JsonConstructor]
    public SavedEaGame(string id, string name, long lastOpened, string gameId) : base(id, name, null, lastOpened)
    {
        GameId = gameId;
    }

    public SavedEaGame(InstalledEaGame game) : base(
        Guid.NewGuid().ToString(),
        game.Name,
        (game.BitmapIcon as BitmapSource)?.ToBase64(),
        Time.UnixTimeMillis)
    {
        GameId = game.GameId;
    }

    public override SpecificItemCategory SpecificCategory => SpecificItemCategory.EaGame;

    protected override void OpenProtected() => throw new NotImplementedException();
}
