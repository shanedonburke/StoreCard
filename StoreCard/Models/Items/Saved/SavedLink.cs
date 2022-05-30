#region

using System.Diagnostics;
using System.Windows.Media.Imaging;
using StoreCard.Static;
using StoreCard.Utils;

#endregion

namespace StoreCard.Models.Items.Saved;

/// <summary>
/// Represents a saved internet link.
/// </summary>
public class SavedLink : SavedItem
{
    /// <summary>
    /// Args that cover launching in private/incognito for most browsers
    /// </summary>
    private const string PrivateWindowArgs = @" -private -incognito -private-window -inprivate";

    /// <summary>
    /// Whether the link should be opened in a private/incognito window.
    /// </summary>
    public bool ShouldOpenPrivate;

    public readonly string Url;

    public SavedLink(
        string id,
        string name,
        string? base64Icon,
        string url,
        long lastOpened,
        bool shouldOpenPrivate) : base(
        id, name,
        base64Icon,
        lastOpened)
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
                Arguments = Url + (ShouldOpenPrivate ? PrivateWindowArgs : string.Empty),
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
