using System;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using StoreCard.Models.Items.Installed;
using StoreCard.Utils;

namespace StoreCard.Models.Items.Saved;

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
        Images.ImageToBase64((BitmapSource) installedApplication.BitmapIcon))
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