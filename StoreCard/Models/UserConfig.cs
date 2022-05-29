#region

using System.Windows.Input;
using Newtonsoft.Json;
using StoreCard.Utils;

#endregion

namespace StoreCard.Models;

public class UserConfig
{
    public static readonly uint DefaultHotKeyModifiers = HotKeyUtils.ModifiersToHotKeyByte(Key.LWin, Key.LeftShift);

    public static readonly uint DefaultVirtualHotKey = HotKeyUtils.KeyToVirtualKey(Key.Z);

    private string _theme = "Lake (Dark)";

    public uint HotKeyModifiers;

    public bool ShouldShowPrefixIcons = false;

    public uint VirtualHotKey;

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

    public string Theme
    {
        get => _theme;
        set
        {
            _theme = value;
            ThemeUtils.SetTheme(value);
        }
    }

    public bool ShouldShowTutorial { get; private set; } = true;

    public void ResetHotKeyToDefault()
    {
        HotKeyModifiers = DefaultHotKeyModifiers;
        VirtualHotKey = DefaultVirtualHotKey;
    }

    public void DisableTutorial() => ShouldShowTutorial = false;
}
