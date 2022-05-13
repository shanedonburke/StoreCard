﻿using System.Windows.Input;
using Newtonsoft.Json;
using StoreCard.Utils;

namespace StoreCard.Models;

internal class UserConfig
{
    public uint HotKeyModifiers;
    public uint VirtualHotKey;

    public UserConfig()
    {
        HotKeyModifiers = HotKeys.ModifiersToHotKeyByte(Key.LWin, Key.LeftShift);
        VirtualHotKey = HotKeys.KeyToVirtualKey(Key.Z);
    }

    [JsonConstructor]
    public UserConfig(uint hotKeyModifiers, uint virtualHotKey)
    {
        HotKeyModifiers = hotKeyModifiers;
        VirtualHotKey = virtualHotKey;
    }
}