using System;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using StoreCard.Models.Items.Installed;
using StoreCard.Static;
using StoreCard.Utils;

namespace StoreCard.Models.Items.Saved;

internal class SavedApplication : SavedItem
{
    public readonly string AppUserModelId;

    [JsonConstructor]
    public SavedApplication(string id, string name, string base64Icon, string appUserModelId, long lastOpened) : base(
        id, name, base64Icon, lastOpened)
    {
        AppUserModelId = appUserModelId;
    }

    public SavedApplication(InstalledApplication installedApplication) : base(
        Guid.NewGuid().ToString(),
        installedApplication.Name,
        ImageUtils.ImageToBase64((BitmapSource) installedApplication.BitmapIcon),
        TimeUtils.UnixTimeMillis)
    {
        AppUserModelId = installedApplication.AppUserModelId;
    }

    public override BitmapSource PrefixIcon => Icons.AppIcon;

    public override ItemCategory Category => ItemCategory.App;

    public override SpecificItemCategory SpecificCategory => SpecificItemCategory.App;

    public override string SecondaryText => ItemCategory.App.ToString();

    protected override void OpenProtected()
    {
        // From https://stackoverflow.com/a/57195200
        Process.Start("explorer.exe", @"shell:appsFolder\" + AppUserModelId);
    }
}
