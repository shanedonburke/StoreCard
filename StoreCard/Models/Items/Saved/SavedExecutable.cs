using System.Diagnostics;
using System.IO;
using StoreCard.Windows;

namespace StoreCard.Models.Items.Saved;

internal class SavedExecutable : SavedItem
{
    public string Path { get; }

    public override ItemCategory Category => ItemCategory.App;

    public SavedExecutable(string id, string name, string? base64Icon, string path) : base(id, name, base64Icon)
    {
        Path = path;
    }

    public override void Open()
    {
        if (!File.Exists(Path)) {
            new MissingItemWindow(this).ShowDialog();
            return;
        }

        Process.Start(Path);
    }
}