// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreCard.Models.Items.Saved;

internal class SavedBattleNetGame : SavedGame
{
    public SavedBattleNetGame(string id, string name, string? base64Icon, long lastOpened) : base(id, name, base64Icon, lastOpened)
    {
    }

    public override SpecificItemCategory SpecificCategory => SpecificItemCategory.BattleNetGame;

    protected override void OpenProtected() => throw new NotImplementedException();
}
