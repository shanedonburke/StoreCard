#region

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using StoreCard.Models;
using StoreCard.Native;

#endregion

namespace StoreCard.Utils;

/// <summary>
/// Utilities for working with system hot keys.
/// </summary>
public class HotKeyUtils
{
    /// <summary>
    /// Converts the given modifier keys to a <c>uint</c> representing their combination,
    /// used by Windows APIs. Non-modifier keys are ignored
    /// </summary>
    /// <param name="modifiers">Array of modifier keys</param>
    /// <returns>Modifier key number</returns>
    public static uint ModifiersToHotKeyByte(params Key[] modifiers) =>
        // Each key is a bit
        modifiers.Aggregate<Key, uint>(0, (current, key) => current | ModifierToHotKeyByte(key));

    /// <summary>
    /// Converts the given modifier key to a <c>uint</c> that represents it,
    /// used by Windows APIs.
    /// </summary>
    /// <param name="modifier">A modifier key</param>
    /// <returns>Modifier key number</returns>
    public static uint ModifierToHotKeyByte(Key modifier) =>
        modifier switch
        {
            Key.LeftAlt => (uint)ModifierKeys.Alt,
            Key.RightAlt => (uint)ModifierKeys.Alt,
            Key.LeftCtrl => (uint)ModifierKeys.Control,
            Key.RightCtrl => (uint)ModifierKeys.Control,
            Key.LeftShift => (uint)ModifierKeys.Shift,
            Key.RightShift => (uint)ModifierKeys.Shift,
            Key.LWin => (uint)ModifierKeys.Windows,
            Key.RWin => (uint)ModifierKeys.Windows,
            // Not a modifier key
            _ => 0
        };

    /// <summary>
    /// Converts a <c>uint</c> representing some combination of modifier keys
    /// (used by Windows APIs) to a list of <c>Key</c> objects.
    /// </summary>
    /// <param name="mod"></param>
    /// <returns></returns>
    public static List<Key> HotKeyByteToModifiers(uint mod)
    {
        List<Key> keys = new();

        // Each key is a bit
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

    /// <summary>
    /// Converts a <c>Key</c> to a <c>uint</c> representing its virtual key
    /// (needed for some Windows APIs).
    /// </summary>
    /// <param name="key">Key to convert</param>
    /// <returns>Virtual key</returns>
    public static uint KeyToVirtualKey(Key key) => (uint)KeyInterop.VirtualKeyFromKey(key);

    /// <summary>
    /// Converts a virtual key to its <c>Key</c> equivalent.
    /// </summary>
    /// <param name="virtualKey">Virtual key</param>
    /// <returns><c>Key</c> equivalent</returns>
    public static Key VirtualKeyToKey(uint virtualKey) => KeyInterop.KeyFromVirtualKey((int)virtualKey);

    /// <summary>
    /// Reads the configured hot key from the given config, then converts it to a display string.
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public static string KeyStringFromConfig(UserConfig config)
    {
        List<Key> allKeys = new();
        allKeys.AddRange(HotKeyByteToModifiers(config.HotKeyModifiers));
        allKeys.Add(VirtualKeyToKey(config.VirtualHotKey));
        return string.Join("+", allKeys.Select(KeyToString));
    }

    /// <summary>
    /// Converts a <c>Key</c> to a friendly display string.
    /// </summary>
    /// <param name="key">Key to convert</param>
    /// <returns>Equivalent string</returns>
    public static string KeyToString(Key key) =>
        key switch
        {
            Key.LeftCtrl => "Ctrl",
            Key.LeftAlt => "Alt",
            Key.LWin => "Win",
            Key.LeftShift => "Shift",
            Key.Tab => "Tab",
            _ => ToAscii(key).ToString().ToUpper()
        };

    /// <summary>
    /// Converts a <c>Key</c> to its ASCII equivalent, e.g., "/" instead of "OemSlash".
    /// From <see href="https://stackoverflow.com/a/736509">this post</see>.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
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
