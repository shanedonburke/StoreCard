using System;
using System.Diagnostics;
using Newtonsoft.Json;
using StoreCard.Models.Items.Installed.Games;
using StoreCard.Static;
using StoreCard.Utils;

namespace StoreCard.Models.Items.Saved.Games;

internal sealed class SavedEpicGame : SavedGame
{
    [JsonConstructor]
    public SavedEpicGame(
        string id,
        string name,
        string base64Icon,
        string appName,
        long lastOpened) : base(id, name, base64Icon, lastOpened)
    {
        AppName = appName;
    }

    public SavedEpicGame(InstalledEpicGame game) : base(
        Guid.NewGuid().ToString(),
        game.Name,
        game.BitmapIcon?.ToBase64(),
        TimeUtils.UnixTimeMillis)
    {
        AppName = game.AppName;
    }

    public string AppName { get; }

    public override SpecificItemCategory SpecificCategory => SpecificItemCategory.EpicGame;

    public override string SecondaryText => GamePlatformNames.EpicGames;

    protected override void OpenProtected()
    {
        Process.Start("CMD.exe", $"/c START com.epicgames.launcher://apps/{AppName}?action=launch&silent=true ");
    }
}
