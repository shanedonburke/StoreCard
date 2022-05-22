using System;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using StoreCard.Native;
using StoreCard.Utils;

namespace StoreCard.Services;

internal class HotKeyService
{
    public static readonly HotKeyService Instance = new();

    private const int HotKeyId = 9000;

    public event Action<string> HotKeyRegistered = delegate { };

    private event Action HotKeyPressed = delegate { };

    private Window? _registeredWindow;

    private HwndSource? _source;

    static HotKeyService()
    {
    }

    private HotKeyService()
    {
    }

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

        var config = AppData.ReadConfigFromFile();
        HotKeyRegistered(HotKeys.KeyStringFromConfig(config));

        return User32.RegisterHotKey(helper.Handle, HotKeyId, config.HotKeyModifiers, config.VirtualHotKey);
    }

    private void UnregisterHotKey()
    {
        if (_registeredWindow == null) return;
        var helper = new WindowInteropHelper(_registeredWindow);

        _source?.RemoveHook(HwndHook);
        _source = null;

        User32.UnregisterHotKey(helper.Handle, HotKeyId);
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
