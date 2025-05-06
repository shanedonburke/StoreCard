#region

using System.Windows.Input;
using Newtonsoft.Json;
using StoreCard.Utils;

#endregion

namespace StoreCard.Models;

/// <summary>
/// Represents the user's configuration of StoreCard.
/// </summary>
public class UserConfig
{
    /// <summary>
    /// The modifier keys of the default hot key (Alt+Q).
    /// </summary>
    public static readonly uint DefaultHotKeyModifiers = HotKeyUtils.ModifiersToHotKeyByte(Key.LeftAlt);

    /// <summary>
    /// The action key of the default hot key (Alt+Q).
    /// </summary>
    public static readonly uint DefaultVirtualHotKey = HotKeyUtils.KeyToVirtualKey(Key.Q);

    private string _theme = "Lake (Dark)";

    /// <summary>
    /// Modifier keys for the current hot key.
    /// </summary>
    public uint HotKeyModifiers;

    /// <summary>
    /// Virtual key for the current hot key (e.g., the virtual hot key for 'Z').
    /// </summary>
    public uint VirtualHotKey;

    /// <summary>
    /// Whether prefix icons should be shown next to saved items.
    /// </summary>
    public bool ShouldShowPrefixIcons = false;

    public UserConfig()
    {
        HotKeyModifiers = DefaultHotKeyModifiers;
        VirtualHotKey = DefaultVirtualHotKey;
    }

    [JsonConstructor]
    public UserConfig(uint hotKeyModifiers, uint virtualHotKey, bool shouldShowTutorial)
    {
        HotKeyModifiers = hotKeyModifiers;
        VirtualHotKey = virtualHotKey;
        ShouldShowTutorial = shouldShowTutorial;
    }

    /// <summary>
    /// Name of the theme. E.g. "Lake (Dark)" for <c>Lake (Dark).xaml</c>.
    /// </summary>
    public string Theme
    {
        get => _theme;
        set
        {
            _theme = value;
            ThemeUtils.SetTheme(value);
        }
    }

    /// <summary>
    /// Whether the tutorial window should be shown on startup. Will be true
    /// following installation until the user clicks "Don't show again".
    /// </summary>
    public bool ShouldShowTutorial { get; private set; } = true;

    /// <summary>
    /// Reset the hot key to its default.
    /// </summary>
    public void ResetHotKeyToDefault()
    {
        HotKeyModifiers = DefaultHotKeyModifiers;
        VirtualHotKey = DefaultVirtualHotKey;
    }

    /// <summary>
    /// Disable the tutorial window for future startups.
    /// </summary>
    public void DisableTutorial() => ShouldShowTutorial = false;
}
