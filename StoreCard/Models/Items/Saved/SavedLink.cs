using System.Diagnostics;

namespace StoreCard.Models.Items.Saved;

internal class SavedLink : SavedItem
{
    public string Url { get; }

    public SavedLink(string id, string name, string? base64Icon, string url) : base(id, name, base64Icon)
    {
        Url = url;
    }

    public override ItemCategory Category => ItemCategory.Link;
    public override SpecificItemCategory SpecificCategory => SpecificItemCategory.Link;

    public override void Open()
    {
        var psi = new ProcessStartInfo
        {
            FileName = Url,
            UseShellExecute = true
        };
        Process.Start(psi);
    }
}