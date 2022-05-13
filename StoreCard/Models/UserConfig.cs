using System.Windows.Input;
using Newtonsoft.Json;
using StoreCard.Utils;

namespace StoreCard.Models;

internal class UserConfig
{
    public static readonly uint DefaultHotKeyModifiers = HotKeys.ModifiersToHotKeyByte(Key.LWin, Key.LeftShift);
    public static readonly uint DefaultVirtualHotKey = HotKeys.KeyToVirtualKey(Key.Z);

    public uint HotKeyModifiers;
    public uint VirtualHotKey;

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

    public void ResetHotKeyToDefault()
    {
        HotKeyModifiers = DefaultHotKeyModifiers;
        VirtualHotKey = DefaultVirtualHotKey;
    }
}