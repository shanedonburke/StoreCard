using System;
using System.Diagnostics;
using System.Linq;
using StoreCard.Commands;
using StoreCard.Windows;

namespace StoreCard.Models.Items.Saved;

public abstract class SavedFileSystemItem : SavedItem
{
    public static string DEFAULT_EXECUTABLE =
        Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\explorer.exe";

    public readonly string Path;

    public string ExecutablePath { get; private set; }

    public string ExecutableName =>
        ExecutablePath == DEFAULT_EXECUTABLE ? "Default" : ExecutablePath.Split(@"\").Last();

    protected SavedFileSystemItem(string id, string name, string? base64Icon, string path, string executablePath)
        : base(id, name, base64Icon)
    {
        Path = path;
        ExecutablePath = executablePath;
    }

    public override void Open()
    {
        if (!Exists())
        {
            new ShowMissingItemAlertCommand(this, () => new EditFileCommand(this).Execute()).Execute();
            return;
        }

        using var openProcess = new Process();

        openProcess.StartInfo.FileName = ExecutablePath;
        openProcess.StartInfo.Arguments = $"\"{Path}\"";
        openProcess.Start();
    }

    public void SetExecutablePath(string path)
    {
        ExecutablePath = path.StartsWith("::") ? DEFAULT_EXECUTABLE : path;
    }

    public abstract bool Exists();
}