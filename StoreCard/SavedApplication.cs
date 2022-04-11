using System.Diagnostics;
using Newtonsoft.Json;

namespace StoreCard;

internal class SavedApplication : SavedItem
{
    [JsonConstructor]
    public SavedApplication(string name, string base64Icon, string appUserModelId, string? executablePath) : base(name, base64Icon)
    {
        AppUserModelId = appUserModelId;
        ExecutablePath = executablePath;
    }

    public SavedApplication(InstalledApplication installedApplication) : base(
        installedApplication.Name,
        ImageUtils.ImageToBase64(installedApplication.BitmapIcon))
    {
        AppUserModelId = installedApplication.AppUserModelId;
        ExecutablePath = installedApplication.ExecutablePath;
    }

    public string AppUserModelId { get; }

    public string? ExecutablePath { get; }

    public override ItemCategory Category => ItemCategory.App;

    public override void Open()
    {
        if (ExecutablePath == null)
        {
            // From https://stackoverflow.com/a/57195200
            Process.Start("explorer.exe", @"shell:appsFolder\" + AppUserModelId);
        }
        else
        {
            Process.Start(ExecutablePath);
        }
    }
}