#region

using System;
using System.Windows;
using Newtonsoft.Json;
using StoreCard.GameLibraries.Itch;
using StoreCard.Models.Items.Installed.Games;
using StoreCard.Services;
using StoreCard.Static;
using StoreCard.Utils;

#endregion

namespace StoreCard.Models.Items.Saved.Games;

/// <summary>
/// Represents a saved itch game.
/// </summary>
public sealed class SavedItchGame : SavedGame
{
    /// <summary>
    /// Cave ID used to launch the game.
    /// </summary>
    public readonly string CaveId;

    [JsonConstructor]
    public SavedItchGame(
        string id,
        string name,
        string? base64Icon,
        long lastOpened,
        string caveId) : base(
        id,
        name,
        base64Icon,
        lastOpened) =>
        CaveId = caveId;

    public SavedItchGame(InstalledItchGame game) : this(
        Guid.NewGuid().ToString(),
        game.Name,
        game.BitmapIcon?.ToBase64(),
        TimeUtils.UnixTimeMillis,
        game.CaveId)
    {
    }

    public override string SecondaryText => GamePlatformNames.Itch;

    public override SpecificItemCategory SpecificCategory => SpecificItemCategory.ItchGame;

    protected override void OpenProtected()
    {
        ButlerClient client = new();
        client.Start();

        if (!client.Authenticate())
        {
            MessageBoxService.Instance.ShowMessageBox(
                "Failed to authenticate. Please verify your itch installation.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            return;
        }

        client.Launch(CaveId);
    }
}
