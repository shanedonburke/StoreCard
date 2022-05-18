using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StoreCard.Static;
using StoreCard.Utils;

namespace StoreCard.Models;

internal class UserConfig
{
    public static readonly uint DefaultHotKeyModifiers = HotKeys.ModifiersToHotKeyByte(Key.LWin, Key.LeftShift);

    public static readonly uint DefaultVirtualHotKey = HotKeys.KeyToVirtualKey(Key.Z);

    public uint HotKeyModifiers;

    public uint VirtualHotKey;

    private string _theme = "Mint (Dark)";

    public UserConfig()
    {
        HotKeyModifiers = DefaultHotKeyModifiers;
        VirtualHotKey = DefaultVirtualHotKey;
    }

    [JsonConstructor]
    public UserConfig(uint hotKeyModifiers, uint virtualHotKey)
    {
        HotKeyModifiers = hotKeyModifiers;
        VirtualHotKey = virtualHotKey;
    }

    public string Theme
    {
        get => _theme;
        set
        {
            _theme = value;
            ((App)Application.Current).SetTheme(value);
        }
    }

    public void ResetHotKeyToDefault()
    {
        HotKeyModifiers = DefaultHotKeyModifiers;
        VirtualHotKey = DefaultVirtualHotKey;
    }
}
