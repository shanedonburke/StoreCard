#region

using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StoreCard.Commands;
using StoreCard.Static;
using StoreCard.Utils;

#endregion

namespace StoreCard.Models.Items.Saved.FileSystem;

/// <summary>
/// Represents a saved file or folder.
/// </summary>
public abstract class SavedFileSystemItem : SavedItem
{
    /// <summary>
    /// By default, open the item in Explorer.
    /// </summary>
    public static string DefaultExecutable => Path.Combine(FolderPaths.Windows, "explorer.exe");

    private string _itemPath;

    protected SavedFileSystemItem(
        string id,
        string name,
        string? base64Icon,
        string itemPath,
        string executablePath,
        long lastOpened)
        : base(id, name, base64Icon, lastOpened)
    {
        _itemPath = itemPath;
        ExecutablePath = executablePath;
    }

    /// <summary>
    /// Path to the executable that should be used to open this item.
    /// E.g., notepad.exe for txt files. Note that Explorer (the default) will take
    /// care of this by using the default app for this file type.
    /// </summary>
    public string ExecutablePath { get; private set; }

    /// <summary>
    /// Absolute path to the item.
    /// </summary>
    public string ItemPath
    {
        get => _itemPath;
        set
        {
            _itemPath = value;
            // The new path may have a different icon
            RegenerateBase64Icon();
        }
    }

    /// <summary>
    /// Display name of <see cref="ExecutablePath"/>.
    /// </summary>
    public string ExecutableName => ExecutablePath == DefaultExecutable
        ? "Default"
        : ExecutablePath.Split(@"\").Last();

    /// <summary>
    /// Whether the item exists in the file system.
    /// </summary>
    /// <returns>A boolean</returns>
    public abstract bool Exists();

    /// <summary>
    /// Sets the path of the executable that should be used to open this item.
    /// If the path starts with "::", it's a special to Explorer.
    /// </summary>
    /// <param name="path">Absolute path</param>
    public void SetExecutablePath(string path) => ExecutablePath = path.StartsWith("::") ? DefaultExecutable : path;

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

        // Most programs know to open a file if you make it the first argument
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
            // Admin privileges needed
            new ShowPrivilegeAlertCommand(
                this,
                ExecutableName,
                () => new ChangeExecutableCommand(this, false).Execute()).Execute();
        }
    }

    /// <summary>
    /// Gets the icon that should be displayed for this item.
    /// </summary>
    /// <returns>The icon</returns>
    protected abstract ImageSource? GetSystemIcon();

    /// <summary>
    /// Sets <see cref="SavedItem.Base64Icon"/> to the encoding of the
    /// current system icon (based on the item path). Called when
    /// a new file path is set.
    /// </summary>
    private void RegenerateBase64Icon()
    {
        if (GetSystemIcon() is BitmapSource bitmapSource)
        {
            Base64Icon = ImageUtils.ImageToBase64(bitmapSource);
        }
    }
}
