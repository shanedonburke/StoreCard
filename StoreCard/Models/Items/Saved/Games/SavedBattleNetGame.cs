#region

using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using StoreCard.GameLibraries.BattleNet;
using StoreCard.Models.Items.Installed.Games;
using StoreCard.Services;
using StoreCard.Utils;

#endregion

namespace StoreCard.Models.Items.Saved.Games;

public sealed class SavedBattleNetGame : SavedGame
{
    public readonly string GameId;

    [JsonConstructor]
    public SavedBattleNetGame(string id, string name, string? base64Icon, long lastOpened, string gameId)
        : base(id, name, base64Icon, lastOpened) =>
        GameId = gameId;

    public SavedBattleNetGame(InstalledBattleNetGame game) : this(
        Guid.NewGuid().ToString(),
        game.Name,
        game.BitmapIcon?.ToBase64(),
        TimeUtils.UnixTimeMillis,
        game.GameId)
    {
    }

    public override SpecificItemCategory SpecificCategory => SpecificItemCategory.BattleNetGame;

    protected override void OpenProtected()
    {
        if (BattleNetLibrary.BattleNetInstallFolder == null)
        {
            MessageBoxService.Instance.ShowMessageBox(
                "The Battle.net installation folder could not be found.", "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            return;
        }

        if (!ProcessUtils.IsProcessWithNameRunning("Battle.net"))
        {
            MessageBoxService.Instance.ShowMessageBox(
                "The Battle.net launcher is not running. Please open it, then try again.", "Warning",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        string battleNetExecPath = Path.Combine(BattleNetLibrary.BattleNetInstallFolder, "Battle.net.exe");

        if (!File.Exists(battleNetExecPath))
        {
            Logger.Log($"The Battle.net executable does not exist (at {battleNetExecPath}).");
            return;
        }

        Process.Start(battleNetExecPath, $"--exec=\"launch {GameId}\"");
    }
}
