using System.Diagnostics;
using System.IO;
using StoreCard.Commands;

namespace StoreCard.Models.Items.Saved;

public class SavedExecutable : SavedItem
{
    public string Path { get; }

    public SavedExecutable(string id, string name, string? base64Icon, string path, long lastOpened) : base(id, name, base64Icon, lastOpened)
    {
        Path = path;
    }

    public override ItemCategory Category => ItemCategory.App;

    public override SpecificItemCategory SpecificCategory => SpecificItemCategory.Executable;

    public override string SecondaryText => ItemCategory.App.ToString();

    protected override void OpenProtected()
    {
        if (!File.Exists(Path))
        {
            new ShowMissingItemAlertCommand(this, () => new EditExecutableCommand(this).Execute()).Execute();
            return;
        }

        Process.Start(Path);
    }
}