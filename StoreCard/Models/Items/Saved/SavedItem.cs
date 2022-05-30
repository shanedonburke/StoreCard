#region

using System;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using StoreCard.Utils;
using static System.String;

#endregion

namespace StoreCard.Models.Items.Saved;

/// <summary>
/// Represents the category of an item. Used to filter
/// items when searching as well as to provide item-specific
/// context menus.
/// </summary>
public enum ItemCategory : uint
{
    None,
    Recent,
    App,
    Game,
    File,
    Folder,
    Link
}

/// <summary>
/// Represents the specific category of an item. Used when distinctions
/// must be made between files and folders, apps and executables, etc.
/// </summary>
public enum SpecificItemCategory : uint
{
    App,
    Executable,
    SteamGame,
    EpicGame,
    BattleNetGame,
    EaGame,
    ItchGame,
    Folder,
    File,
    Link
}

/// <summary>
/// Represents a saved item of any kind.
/// </summary>
public abstract class SavedItem : IListBoxItem
{
    public readonly string Id;

    protected SavedItem(string id, string name, string? base64Icon, long lastOpened)
    {
        Id = id;
        Name = name;
        Base64Icon = base64Icon;
        LastOpened = lastOpened;
    }

    /// <summary>
    /// Item icon as a Base64 string.
    /// </summary>
    public string? Base64Icon { get; set; }

    /// <summary>
    /// Epoch time in milliseconds when the item was last opened.
    /// </summary>
    public long LastOpened { get; private set; }

    public abstract ItemCategory Category { get; }

    public abstract SpecificItemCategory SpecificCategory { get; }

    public string Name { get; set; }

    [JsonIgnore] public BitmapSource? BitmapIcon => Base64Icon != null ? ImageUtils.Base64ToImage(Base64Icon) : null;

    [JsonIgnore] public virtual BitmapSource? PrefixIcon => null;

    [JsonIgnore] public virtual string SecondaryText => Empty;

    public int CompareTo(IListBoxItem? other) => Compare(Name, other?.Name, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Opens the item and updates <see cref="LastOpened"/>.
    /// </summary>
    public void Open()
    {
        LastOpened = TimeUtils.UnixTimeMillis;
        AppData.UpdateSavedItemById<SavedItem>(Id, i => i.LastOpened = LastOpened);
        OpenProtected();
    }

    /// <summary>
    /// Item type-specific logic for opening this item.
    /// </summary>
    protected abstract void OpenProtected();
}
