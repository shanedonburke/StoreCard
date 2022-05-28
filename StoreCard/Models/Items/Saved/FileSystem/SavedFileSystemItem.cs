using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StoreCard.Commands;
using StoreCard.Static;
using StoreCard.Utils;

namespace StoreCard.Models.Items.Saved.FileSystem;

public abstract class SavedFileSystemItem : SavedItem
{
    public static readonly string DefaultExecutable = Path.Combine(FolderPaths.Windows, "explorer.exe");

    public string ExecutablePath { get; private set; }

    private string _itemItemPath;

    protected SavedFileSystemItem(
        string id,
        string name,
        string? base64Icon,
        string itemItemPath,
        string executablePath,
        long lastOpened)
        : base(id, name, base64Icon, lastOpened)
    {
        _itemItemPath = itemItemPath;
        ExecutablePath = executablePath;
    }

    public string ItemPath
    {
        get => _itemItemPath;
        set
        {
            _itemItemPath = value;
            RegenerateBase64Icon();
        }
    }

    public string ExecutableName => ExecutablePath == DefaultExecutable
        ? "Default"
        : ExecutablePath.Split(@"\").Last();

    public abstract bool Exists();

    public void SetExecutablePath(string path)
    {
        ExecutablePath = path.StartsWith("::") ? DefaultExecutable : path;
    }

    protected override void OpenProtected()
    {
        if (!Exists())
        {
            new ShowMissingItemAlertCommand(this, () => new EditFileCommand(this).Execute()).Execute();
            return;
        }

        if (!File.Exists(ExecutablePath))
        {
            new ShowMissingExecutableAlertCommand(this).Execute();
            return;
        }

        using var openProcess = new Process
        {
            StartInfo = new ProcessStartInfo {FileName = ExecutablePath, Arguments = $"\"{ItemPath}\""}
        };

        try
        {
            openProcess.Start();
        }
        catch (Win32Exception)
        {
            new ShowPrivilegeAlertCommand(this).Execute();
        }
    }

    protected abstract ImageSource? GetSystemIcon();

    private void RegenerateBase64Icon()
    {
        if (GetSystemIcon() is BitmapSource bitmapSource)
        {
            Base64Icon = ImageUtils.ImageToBase64(bitmapSource);
        }
    }
}
