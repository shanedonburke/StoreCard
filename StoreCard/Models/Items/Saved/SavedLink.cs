using System.Diagnostics;

namespace StoreCard.Models.Items.Saved;

public class SavedLink : SavedItem
{
    public string Url { get; }

    public SavedLink(string id, string name, string? base64Icon, string url, long lastOpened) : base(id, name,
        base64Icon, lastOpened)
    {
        Url = url;
    }

    public override ItemCategory Category => ItemCategory.Link;
    public override SpecificItemCategory SpecificCategory => SpecificItemCategory.Link;

    protected override void OpenProtected()
    {
        var psi = new ProcessStartInfo
        {
            FileName = Url,
            UseShellExecute = true
        };
        Process.Start(psi);
    }
}