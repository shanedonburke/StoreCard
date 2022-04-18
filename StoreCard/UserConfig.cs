using System.Windows.Input;
using Newtonsoft.Json;

namespace StoreCard
{
    internal class UserConfig
    {
        public uint HotKeyModifiers;
        public uint VirtualHotKey;

        public UserConfig()
        {
            HotKeyModifiers = HotKeyUtils.ModifiersToHotKeyByte(Key.LWin, Key.LeftShift);
            VirtualHotKey = HotKeyUtils.KeyToVirtualKey(Key.Z);
        }

        [JsonConstructor]
        public UserConfig(uint hotKeyModifiers, uint virtualHotKey)
        {
            HotKeyModifiers = hotKeyModifiers;
            VirtualHotKey = virtualHotKey;
        }
    }
}
