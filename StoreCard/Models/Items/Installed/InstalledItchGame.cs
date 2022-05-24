// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using StoreCard.Models.Items.Saved;
using StoreCard.Static;

namespace StoreCard.Models.Items.Installed;

internal class InstalledItchGame : InstalledGame
{
    public readonly string CaveId;

    public InstalledItchGame(string name, BitmapSource? bitmapIcon, string caveId) : base(name, bitmapIcon)
    {
        CaveId = caveId;
    }

    public override string SecondaryText => GamePlatformNames.Itch;

    public override SavedItem SavedItem => new SavedItchGame(this);
}
