#region

using System;
using System.Diagnostics;
using Newtonsoft.Json;
using StoreCard.Models.Items.Installed.Games;
using StoreCard.Static;
using StoreCard.Utils;

#endregion

namespace StoreCard.Models.Items.Saved.Games;

/// <summary>
/// Represents a saved Epic Games game.
/// </summary>
public sealed class SavedEpicGame : SavedGame
{
    /// <summary>
    /// App name used to launch the game.
    /// </summary>
    public readonly string AppName;

    [JsonConstructor]
    public SavedEpicGame(
        string id,
        string name,
        string base64Icon,
        string appName,
        long lastOpened) : base(id,
        name,
        base64Icon,
        lastOpened) =>
        AppName = appName;

    public SavedEpicGame(InstalledEpicGame game) : base(
        Guid.NewGuid().ToString(),
        game.Name,
        game.BitmapIcon?.ToBase64(),
        TimeUtils.UnixTimeMillis) =>
        AppName = game.AppName;

    public override SpecificItemCategory SpecificCategory => SpecificItemCategory.EpicGame;

    public override string SecondaryText => GamePlatformNames.EpicGames;

    protected override void OpenProtected() => Process.Start("CMD.exe",
        $"/c START com.epicgames.launcher://apps/{AppName}?action=launch&silent=true ");
}
