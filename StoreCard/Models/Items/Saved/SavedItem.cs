#region

using System;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using StoreCard.Utils;
using static System.String;

#endregion

namespace StoreCard.Models.Items.Saved;

public enum ItemCategory : uint
{
    None,
    Recent,
    App,
    Game,
    Folder,
    File,
    Link
}

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

    public string? Base64Icon { get; set; }

    public long LastOpened { get; private set; }

    public abstract ItemCategory Category { get; }

    public abstract SpecificItemCategory SpecificCategory { get; }

    public string Name { get; set; }

    [JsonIgnore] public BitmapSource? BitmapIcon => Base64Icon != null ? ImageUtils.Base64ToImage(Base64Icon) : null;

    [JsonIgnore] public virtual BitmapSource? PrefixIcon => null;

    [JsonIgnore] public virtual string SecondaryText => Empty;

    public int CompareTo(IListBoxItem? other) => Compare(Name, other?.Name, StringComparison.OrdinalIgnoreCase);

    public void Open()
    {
        LastOpened = TimeUtils.UnixTimeMillis;
        AppData.UpdateSavedItemById<SavedItem>(Id, i => i.LastOpened = LastOpened);
        OpenProtected();
    }

    protected abstract void OpenProtected();
}
