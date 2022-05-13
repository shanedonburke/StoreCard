using System;
using System.Windows.Media;
using Newtonsoft.Json;
using StoreCard.Utils;
using static System.String;

namespace StoreCard.Models.Items.Saved;

public enum ItemCategory : uint
{
    None,
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
    Folder,
    File,
    Link
}

public abstract class SavedItem : IListBoxItem
{
    public readonly string Id;

    public string Name { get; set; }

    public string? Base64Icon { get; protected set; }

    public abstract ItemCategory Category { get; }

    public abstract SpecificItemCategory SpecificCategory { get; }

    [JsonIgnore] public ImageSource? BitmapIcon => Base64Icon != null ? Images.Base64ToImage(Base64Icon) : null;

    [JsonIgnore] public virtual ImageSource? PrefixIcon => null;

    [JsonIgnore] public virtual string SecondaryText => Empty;

    protected SavedItem(string id, string name, string? base64Icon)
    {
        Id = id;
        Name = name;
        Base64Icon = base64Icon;
    }

    public abstract void Open();

    public int CompareTo(IListBoxItem? other)
    {
        return Compare(Name, other?.Name, StringComparison.OrdinalIgnoreCase);
    }
}