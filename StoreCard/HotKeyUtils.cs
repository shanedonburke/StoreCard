using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StoreCard
{
    internal class HotKeyUtils
    {
        private const uint ModAlt = 0x0001;
        private const uint ModControl = 0x0002;
        private const uint ModShift = 0x0004;
        private const uint ModWin = 0x0008;

        public static uint ModifiersToHotKeyByte(params Key[] modifiers)
        {
            return modifiers.Aggregate<Key, uint>(0, (current, key) => current | ModifierToHotKeyByte(key));
        }

        public static uint ModifierToHotKeyByte(Key modifier)
        {
            return modifier switch
            {
                Key.LeftAlt => ModAlt,
                Key.RightAlt => ModAlt,
                Key.LeftCtrl => ModControl,
                Key.RightCtrl => ModControl,
                Key.LeftShift => ModShift,
                Key.RightShift => ModShift,
                Key.LWin => ModWin,
                Key.RWin => ModWin,
                _ => 0
            };
        }

        public static List<Key> HotKeyByteToModifiers(uint mod)
        {
            List<Key> keys = new();

            if ((mod & ModControl) != 0)
            {
                keys.Add(Key.LeftCtrl);
            }

            if ((mod & ModAlt) != 0)
            {
                keys.Add(Key.LeftAlt);
            }

            if ((mod & ModWin) != 0)
            {
                keys.Add(Key.LWin);
            }
            
            if ((mod & ModShift) != 0)
            {
                keys.Add(Key.LeftShift);
            }

            return keys;
        }

        public static uint KeyToVirtualKey(Key key)
        {
            return (uint) KeyInterop.VirtualKeyFromKey(key);
        }

        public static Key VirtualKeyToKey(uint virtualKey)
        {
            return KeyInterop.KeyFromVirtualKey((int) virtualKey);
        }

        public static string KeyStringFromConfig(UserConfig config)
        {
            List<Key> allKeys = new();
            allKeys.AddRange(HotKeyByteToModifiers(config.HotKeyModifiers));
            allKeys.Add(VirtualKeyToKey(config.VirtualHotKey));
            return string.Join("+", allKeys.Select(KeyToString));
        }

        private static string KeyToString(Key key) {
            return key switch {
                Key.LeftCtrl => "Ctrl",
                Key.LeftAlt => "Alt",
                Key.LWin => "Win",
                Key.LeftShift => "Shift",
                _ => key.ToString()
            };
        }
    }
}
