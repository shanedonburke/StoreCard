#region

using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using StoreCard.Models.Items.Installed;
using StoreCard.Static;
using StoreCard.Utils;

#endregion

namespace StoreCard.Models.Items.Saved;

/// <summary>
/// Represents a saved app.
/// </summary>
public sealed class SavedApp : SavedItem
{
    /// <summary>
    /// AUMID used to launch the app.
    /// </summary>
    public readonly string AppUserModelId;

    /// <summary>
    /// Windows detects Battle.net games as apps, so we represent them as one
    /// with this flag.
    /// </summary>
    public readonly bool IsBattleNetGame;

    [JsonConstructor]
    public SavedApp(
        string id,
        string name,
        string base64Icon,
        string appUserModelId,
        bool isBattleNetGame,
        long lastOpened) : base(id,
        name,
        base64Icon,
        lastOpened)
    {
        AppUserModelId = appUserModelId;
        IsBattleNetGame = isBattleNetGame;
    }

    public SavedApp(InstalledApp installedApp) : this(
        Guid.NewGuid().ToString(),
        installedApp.Name,
        ImageUtils.ImageToBase64(installedApp.BitmapIcon),
        installedApp.AppUserModelId,
        installedApp.IsBattleNetGame,
        TimeUtils.UnixTimeMillis)
    {
    }

    public override BitmapSource PrefixIcon => Icons.AppIcon;

    public override ItemCategory Category => ItemCategory.App;

    public override SpecificItemCategory SpecificCategory => SpecificItemCategory.App;

    public override string SecondaryText => IsBattleNetGame ? GamePlatformNames.BattleNet : ItemCategory.App.ToString();

    // From https://stackoverflow.com/a/57195200
    protected override void OpenProtected() =>
        Process.Start("explorer.exe", @"shell:appsFolder\" + AppUserModelId);
}
