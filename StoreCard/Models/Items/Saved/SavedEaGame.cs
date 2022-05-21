﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.Windows;
using Newtonsoft.Json;
using StoreCard.GameLibraries;
using StoreCard.Models.Items.Installed;
using StoreCard.Static;
using StoreCard.Utils;

namespace StoreCard.Models.Items.Saved;

internal class SavedEaGame : SavedGame
{
    public readonly string AppId;

    [JsonConstructor]
    public SavedEaGame(string id, string name, long lastOpened, string appId) : base(id, name, null, lastOpened)
    {
        AppId = appId;
    }

    public SavedEaGame(InstalledEaGame game) : base(
        Guid.NewGuid().ToString(),
        game.Name,
        game.BitmapIcon?.ToBase64(),
        Time.UnixTimeMillis)
    {
        AppId = game.GameId;
    }

    public override SpecificItemCategory SpecificCategory => SpecificItemCategory.EaGame;

    public override string SecondaryText => GamePlatformNames.Ea;

    protected override void OpenProtected()
    {
        if (EaLibrary.EaLauncherPath == null)
        {
            MessageBox.Show("The EA launcher could not be found.");
            return;
        }

        Process.Start(EaLibrary.EaLauncherPath, $"origin://LaunchGame/{AppId}");
    }
}
