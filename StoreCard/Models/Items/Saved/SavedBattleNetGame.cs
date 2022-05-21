using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using StoreCard.GameLibraries.BattleNet;
using StoreCard.Models.Items.Installed;
using StoreCard.Utils;
using File = System.IO.File;

namespace StoreCard.Models.Items.Saved;

internal class SavedBattleNetGame : SavedGame
{
    public readonly string GameId;

    [JsonConstructor]
    public SavedBattleNetGame(string id, string name, string? base64Icon, long lastOpened, string gameId) : base(id, name, base64Icon, lastOpened)
    {
        GameId = gameId;
    }

    public SavedBattleNetGame(InstalledBattleNetGame game) : this(
        Guid.NewGuid().ToString(),
        game.Name,
        game.BitmapIcon?.ToBase64(),
        Time.UnixTimeMillis,
        game.GameId)
    {
    }

    public override SpecificItemCategory SpecificCategory => SpecificItemCategory.BattleNetGame;

    protected override void OpenProtected()
    {
        if (BattleNetLibrary.BattleNetInstallFolder == null)
        {
            MessageBox.Show("The Battle.net installation folder could not be found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        string battleNetExecPath = Path.Combine(BattleNetLibrary.BattleNetInstallFolder, "Battle.net.exe");
        if (!File.Exists(battleNetExecPath)) return;

        Process.Start(battleNetExecPath, $"--exec=\"launch {GameId}\"");
    }
}
