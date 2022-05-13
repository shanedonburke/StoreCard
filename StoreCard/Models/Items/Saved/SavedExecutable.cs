﻿using System.Diagnostics;
using System.IO;
using StoreCard.Commands;
using StoreCard.Windows;

namespace StoreCard.Models.Items.Saved;

public class SavedExecutable : SavedItem
{
    public string Path { get; }

    public override ItemCategory Category => ItemCategory.App;
    public override SpecificItemCategory SpecificCategory => SpecificItemCategory.Executable;

    public SavedExecutable(string id, string name, string? base64Icon, string path) : base(id, name, base64Icon)
    {
        Path = path;
    }

    public override void Open()
    {
        if (!File.Exists(Path))
        {
            new ShowMissingItemAlertCommand(this, () => new EditExecutableCommand(this).Execute()).Execute();
            return;
        }

        Process.Start(Path);
    }
}