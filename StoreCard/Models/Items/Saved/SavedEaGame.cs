﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.Windows;
using Newtonsoft.Json;
using StoreCard.GameLibraries;
using StoreCard.GameLibraries.Ea;
using StoreCard.Models.Items.Installed;
using StoreCard.Services;
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

    public SavedEaGame(InstalledEaGame game) : this(
        Guid.NewGuid().ToString(),
        game.Name,
        Time.UnixTimeMillis,
        game.AppId)
    {
    }

    public override SpecificItemCategory SpecificCategory => SpecificItemCategory.EaGame;

    public override string SecondaryText => GamePlatformNames.Ea;

    protected override void OpenProtected()
    {
        if (EaLibrary.EaLauncherPath == null)
        {
            MessageBoxService.Instance.ShowMessageBox(
                "The EA launcher could not be found.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            return;
        }

        if (!Processes.IsProcessWithNameRunning("EADesktop"))
        {
            MessageBoxService.Instance.ShowMessageBox(
                "EA Desktop is not running. Please open it, then try again.",
                "Warning",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        Process.Start(EaLibrary.EaLauncherPath, $"origin://LaunchGame/{AppId}");
    }
}
