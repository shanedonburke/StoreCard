using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StoreCard.Commands;
using StoreCard.Utils;

namespace StoreCard.Models.Items.Saved;

public abstract class SavedFileSystemItem : SavedItem
{
    public static string DefaultExecutable =
        Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\explorer.exe";

    public string ExecutablePath { get; private set; }

    private string _path;

    protected SavedFileSystemItem(string id, string name, string? base64Icon, string path, string executablePath, long lastOpened)
        : base(id, name, base64Icon, lastOpened)
    {
        _path = path;
        ExecutablePath = executablePath;
    }

    public string Path
    {
        get => _path;
        set
        {
            _path = value;
            RegenerateBase64Icon();
        }
    }

    public string ExecutableName =>
        ExecutablePath == DefaultExecutable ? "Default" : ExecutablePath.Split(@"\").Last();

    public void SetExecutablePath(string path)
    {
        ExecutablePath = path.StartsWith("::") ? DefaultExecutable : path;
    }

    public abstract bool Exists();

    protected override void OpenProtected()
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

    protected abstract ImageSource? GetSystemIcon();

    private void RegenerateBase64Icon()
    {
        if (GetSystemIcon() is BitmapSource bitmapSource)
        {
            Base64Icon = Images.ImageToBase64(bitmapSource);
        }
    }
}