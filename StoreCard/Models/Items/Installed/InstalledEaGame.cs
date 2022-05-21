// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StoreCard.Models.Items.Saved;

namespace StoreCard.Models.Items.Installed;

internal class InstalledEaGame : InstalledGame
{
    public readonly string GameId;

    public InstalledEaGame(string name, string gameId, BitmapSource? bitmapIcon) : base(name, bitmapIcon)
    {
        GameId = gameId;
    }

    public override SavedItem SavedItem => new SavedEaGame(this);
}
