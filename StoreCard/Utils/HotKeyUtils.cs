using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using StoreCard.Models;
using StoreCard.Native;

namespace StoreCard.Utils;

public class HotKeyUtils
{
    public static uint ModifiersToHotKeyByte(params Key[] modifiers)
    {
        return modifiers.Aggregate<Key, uint>(0, (current, key) => current | ModifierToHotKeyByte(key));
    }

    public static uint ModifierToHotKeyByte(Key modifier)
    {
        return modifier switch
        {
            Key.LeftAlt => (uint)ModifierKeys.Alt,
            Key.RightAlt => (uint)ModifierKeys.Alt,
            Key.LeftCtrl => (uint)ModifierKeys.Control,
            Key.RightCtrl => (uint)ModifierKeys.Control,
            Key.LeftShift => (uint)ModifierKeys.Shift,
            Key.RightShift => (uint)ModifierKeys.Shift,
            Key.LWin => (uint)ModifierKeys.Windows,
            Key.RWin => (uint)ModifierKeys.Windows,
            _ => 0
        };
    }

    public static List<Key> HotKeyByteToModifiers(uint mod)
    {
        List<Key> keys = new();

        if ((mod & (uint)ModifierKeys.Control) != 0)
        {
            keys.Add(Key.LeftCtrl);
        }

        if ((mod & (uint)ModifierKeys.Alt) != 0)
        {
            keys.Add(Key.LeftAlt);
        }

        if ((mod & (uint)ModifierKeys.Windows) != 0)
        {
            keys.Add(Key.LWin);
        }

        if ((mod & (uint)ModifierKeys.Shift) != 0)
        {
            keys.Add(Key.LeftShift);
        }

        return keys;
    }

    public static uint KeyToVirtualKey(Key key)
    {
        return (uint)KeyInterop.VirtualKeyFromKey(key);
    }

    public static Key VirtualKeyToKey(uint virtualKey)
    {
        return KeyInterop.KeyFromVirtualKey((int)virtualKey);
    }

    public static string KeyStringFromConfig(UserConfig config)
    {
        List<Key> allKeys = new();
        allKeys.AddRange(HotKeyByteToModifiers(config.HotKeyModifiers));
        allKeys.Add(VirtualKeyToKey(config.VirtualHotKey));
        return string.Join("+", allKeys.Select(KeyToString));
    }

    public static string KeyToString(Key key)
    {
        return key switch
        {
            Key.LeftCtrl => "Ctrl",
            Key.LeftAlt => "Alt",
            Key.LWin => "Win",
            Key.LeftShift => "Shift",
            Key.Tab => "Tab",
            _ => ToAscii(key).ToString().ToUpper()
        };
    }

    // From https://stackoverflow.com/a/736509
    private static char ToAscii(Key key)
    {
        var outputBuilder = new StringBuilder(2);
        int result = User32.ToAscii(
            KeyToVirtualKey(key),
            0,
            new byte[256],
            outputBuilder,
            0);
        return result == 1 ? outputBuilder[0] : ' ';
    }
}
