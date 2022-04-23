using System;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace StoreCard;

internal class SavedApplication : SavedItem
{
    [JsonConstructor]
    public SavedApplication(string id, string name, string base64Icon, string appUserModelId) : base(id, name, base64Icon)
    {
        AppUserModelId = appUserModelId;
    }

    public SavedApplication(InstalledApplication installedApplication) : base(
        Guid.NewGuid().ToString(),
        installedApplication.Name,
        ImageUtils.ImageToBase64((BitmapSource) installedApplication.BitmapIcon))
    {
        AppUserModelId = installedApplication.AppUserModelId;
    }

    public string AppUserModelId { get; }

    public override ItemCategory Category => ItemCategory.App;

    public override void Open()
    {
        // From https://stackoverflow.com/a/57195200
        Process.Start("explorer.exe", @"shell:appsFolder\" + AppUserModelId);
    }
}