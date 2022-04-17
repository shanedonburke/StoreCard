using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StoreCard
{
    internal class UserConfig
    {
        public uint HotKeyModifiers;
        public uint VirtualHotKey;

        public UserConfig()
        {
            HotKeyModifiers = HotKeyUtils.ModifiersToHotKeyByte(Key.LWin, Key.LeftShift);
            VirtualHotKey = HotKeyUtils.KeyToVirtualKey(Key.X);
        }

        public UserConfig(uint hotKeyModifiers, uint virtualHotKey)
        {
            HotKeyModifiers = hotKeyModifiers;
            VirtualHotKey = virtualHotKey;
        }
    }
}
