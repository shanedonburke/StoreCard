using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using StoreCard.Models;
using StoreCard.Utils;

namespace StoreCard.Services;

internal class HotKeyService
{
    static HotKeyService()
    {
    }

    private HotKeyService()
    {
    }

    public static HotKeyService Instance { get; } = new();

    private const int HotKeyId = 9000;

    public static uint ModifiersToHotKeyByte(params Key[] modifiers) {
        return modifiers.Aggregate<Key, uint>(0, (current, key) => current | ModifierToHotKeyByte(key));
    }

    public static uint ModifierToHotKeyByte(Key modifier) {
        return modifier switch {
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

    public static List<Key> HotKeyByteToModifiers(uint mod) {
        List<Key> keys = new();

        if ((mod & (uint)ModifierKeys.Control) != 0) {
            keys.Add(Key.LeftCtrl);
        }

        if ((mod & (uint)ModifierKeys.Alt) != 0) {
            keys.Add(Key.LeftAlt);
        }

        if ((mod & (uint)ModifierKeys.Windows) != 0) {
            keys.Add(Key.LWin);
        }

        if ((mod & (uint)ModifierKeys.Shift) != 0) {
            keys.Add(Key.LeftShift);
        }

        return keys;
    }

    public static uint KeyToVirtualKey(Key key) {
        return (uint)KeyInterop.VirtualKeyFromKey(key);
    }

    public static Key VirtualKeyToKey(uint virtualKey) {
        return KeyInterop.KeyFromVirtualKey((int)virtualKey);
    }

    public static string KeyStringFromConfig(UserConfig config) {
        List<Key> allKeys = new();
        allKeys.AddRange(HotKeyByteToModifiers(config.HotKeyModifiers));
        allKeys.Add(VirtualKeyToKey(config.VirtualHotKey));
        return string.Join("+", allKeys.Select(KeyToString));
    }

    public static string KeyToString(Key key) {
        return key switch {
            Key.LeftCtrl => "Ctrl",
            Key.LeftAlt => "Alt",
            Key.LWin => "Win",
            Key.LeftShift => "Shift",
            _ => ToAscii(key).ToString().ToUpper()
        };
    }

    // From https://stackoverflow.com/a/736509
    private static char ToAscii(Key key) {
        var outputBuilder = new StringBuilder(2);
        var result = ToAscii(KeyToVirtualKey(key), 0, new byte[256],
            outputBuilder, 0);
        return result == 1 ? outputBuilder[0] : ' ';
    }

    [DllImport("User32.dll")]
    private static extern int ToAscii(uint uVirtKey, uint uScanCode,
        byte[] lpKeyState,
        [Out] StringBuilder lpChar,
        uint uFlags);

    // Hotkey solution from https://stackoverflow.com/a/11378213
    [DllImport("User32.dll")]
    private static extern bool RegisterHotKey(
        [In] IntPtr hWnd,
        [In] int id,
        [In] uint fsModifiers,
        [In] uint vk);

    [DllImport("User32.dll")]
    private static extern bool UnregisterHotKey(
        [In] IntPtr hWnd,
        [In] int id);

    public event Action<string> HotKeyRegistered = delegate { };

    private event Action HotKeyPressed = delegate { };

    private Window? _registeredWindow;

    private HwndSource? _source;

    // See https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-registerhotkey
    // and https://docs.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes
    // for key codes
    public bool RegisterHotKey(Window window, Action hotKeyPressed)
    {
        if (!RegisterHotKey(window)) return false;
        HotKeyPressed += hotKeyPressed;
        return true;

    }

    public void UnregisterHotKey(Action hotKeyPressed)
    {
        UnregisterHotKey();
        HotKeyPressed -= hotKeyPressed;
    }

    public void UpdateHotKey()
    {
        if (_registeredWindow == null) return;
        UnregisterHotKey();
        RegisterHotKey(_registeredWindow);
    }

    private bool RegisterHotKey(Window window)
    {
        _registeredWindow = window;
        var helper = new WindowInteropHelper(window);

        _source = HwndSource.FromHwnd(helper.Handle);
        _source?.AddHook(HwndHook);

        var config = StorageUtils.ReadConfigFromFile();
        HotKeyRegistered(KeyStringFromConfig(config));

        return RegisterHotKey(helper.Handle, HotKeyId, config.HotKeyModifiers, config.VirtualHotKey);
    }

    private void UnregisterHotKey()
    {
        if (_registeredWindow == null) return;
        var helper = new WindowInteropHelper(_registeredWindow);

        _source?.RemoveHook(HwndHook);
        _source = null;

        UnregisterHotKey(helper.Handle, HotKeyId);
    }

    private IntPtr HwndHook(
        IntPtr hwnd,
        int msg,
        IntPtr wParam,
        IntPtr lParam,
        ref bool handled) {
        const int wmHotkey = 0x0312;
        switch (msg) {
            case wmHotkey:
                switch (wParam.ToInt32()) {
                    case HotKeyService.HotKeyId:
                        HotKeyPressed();
                        handled = true;
                        break;
                }

                break;
        }

        return IntPtr.Zero;
    }
}