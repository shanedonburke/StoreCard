using System.Diagnostics;
using System.Windows.Media.Imaging;
using StoreCard.Static;
using StoreCard.Utils;

namespace StoreCard.Models.Items.Saved;

public class SavedLink : SavedItem
{
    public string Url { get; }

    public bool ShouldOpenPrivate;

    public SavedLink(
        string id,
        string name,
        string? base64Icon,
        string url,
        long lastOpened,
        bool shouldOpenPrivate)
        : base(id, name, base64Icon, lastOpened)
    {
        Url = url;
        ShouldOpenPrivate = shouldOpenPrivate;
    }

    public override ItemCategory Category => ItemCategory.Link;

    public override SpecificItemCategory SpecificCategory => SpecificItemCategory.Link;

    public override string SecondaryText => ItemCategory.Link.ToString();

    public override BitmapSource PrefixIcon => Icons.LinkIcon;

    protected override void OpenProtected()
    {
        string? defaultBrowserExe = BrowserUtils.GetDefaultBrowserExecutable();

        ProcessStartInfo psi;

        if (defaultBrowserExe != null)
        {
            psi = new ProcessStartInfo
            {
                FileName = defaultBrowserExe,
                Arguments = Url + (ShouldOpenPrivate ? " -private -incognito -private-window -inprivate" : ""),
                UseShellExecute = true
            };
        }
        else
        {
            Logger.Log("Failed to get default browser. Opening link without executable or options...");
            psi = new ProcessStartInfo {FileName = Url, UseShellExecute = true};
        }

        Process.Start(psi);
    }
}
