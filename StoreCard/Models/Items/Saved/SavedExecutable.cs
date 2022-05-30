#region

using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;
using StoreCard.Commands;
using StoreCard.Static;

#endregion

namespace StoreCard.Models.Items.Saved;

/// <summary>
/// Represents a saved executable (as an app).
/// </summary>
public sealed class SavedExecutable : SavedItem
{
    /// <summary>
    /// Path to the .exe file.
    /// </summary>
    public readonly string Path;

    public SavedExecutable(
        string id,
        string name,
        string? base64Icon,
        string path,
        long lastOpened) : base(id,
        name,
        base64Icon,
        lastOpened) =>
        Path = path;

    public override BitmapSource PrefixIcon => Icons.AppIcon;

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

        try
        {
            Process.Start(Path);
        }
        catch (Win32Exception)
        {
            // Admin privileges required
            new ShowPrivilegeAlertCommand(
                this,
                Name,
                () => new EditExecutableCommand(this).Execute()).Execute();
        }
    }
}
