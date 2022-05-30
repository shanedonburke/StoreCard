#region

using System;
using System.Windows;
using System.Windows.Interop;
using StoreCard.Models;
using StoreCard.Native;
using StoreCard.Utils;

#endregion

namespace StoreCard.Services;

/// <summary>
/// A service to manage hot keys and related functions.
/// </summary>
public sealed class HotKeyService
{
    public static readonly HotKeyService Instance = new();

    /// <summary>
    /// Parameter representing a hot key press.
    /// </summary>
    private const int HotKeyId = 9000;

    /// <summary>
    /// The window that registered the global hot key.
    /// </summary>
    private Window? _registeredWindow;

    private HwndSource? _source;

    static HotKeyService()
    {
    }

    private HotKeyService()
    {
    }

    /// <summary>
    /// Event handler that accepts a hot key string when a hot key is registered.
    /// </summary>
    public event Action<string> HotKeyRegistered = delegate { };

    /// <summary>
    /// Event handler that is triggered when the global hot key is pressed.
    /// </summary>
    private event Action HotKeyPressed = delegate { };

    /// <summary>
    /// Registers a global hot key.
    /// See <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-registerhotkey">here</see>
    /// and <see href="https://docs.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes">here</see>
    /// for key codes.
    /// </summary>
    /// <param name="window">The window registering the hot key</param>
    /// <param name="hotKeyPressed">An action to be invoked when the hot key is pressed.</param>
    /// <returns><c>true</c> if the hot key was registered, otherwise <c>false</c>.</returns>
    public bool RegisterHotKey(Window window, Action hotKeyPressed)
    {
        if (!RegisterHotKey(window))
        {
            return false;
        }

        HotKeyPressed += hotKeyPressed;
        return true;
    }

    /// <summary>
    /// Update the global hot key to reflect the <see cref="UserConfig"/>.
    /// </summary>
    public void UpdateHotKey()
    {
        if (_registeredWindow == null)
        {
            Logger.Log("Tried to update the global hot key before it was registered.");
            return;
        }

        UnregisterHotKey();
        RegisterHotKey(_registeredWindow);
    }

    /// <summary>
    /// Register the global hot key.
    /// </summary>
    /// <param name="window">The window registering the hot key (should be the taskbar icon window).</param>
    /// <returns><c>true</c> if the hot key was registered, otherwise <c>false</c></returns>
    private bool RegisterHotKey(Window window)
    {
        _registeredWindow = window;
        var helper = new WindowInteropHelper(window);

        _source = HwndSource.FromHwnd(helper.Handle);
        _source?.AddHook(HwndHook);

        UserConfig config = AppData.ReadConfigFromFile();


        if (!User32.RegisterHotKey(helper.Handle, HotKeyId, config.HotKeyModifiers, config.VirtualHotKey))
        {
            return false;
        }

        // Call event handler
        HotKeyRegistered(HotKeyUtils.KeyStringFromConfig(config));
        return true;

    }

    /// <summary>
    /// Unregister the current global hot key.
    /// </summary>
    public void UnregisterHotKey()
    {
        if (_registeredWindow == null)
        {
            return;
        }

        var helper = new WindowInteropHelper(_registeredWindow);

        _source?.RemoveHook(HwndHook);
        _source = null;

        User32.UnregisterHotKey(helper.Handle, HotKeyId);

        // Remove all event handlers
        foreach (Delegate d in HotKeyPressed.GetInvocationList())
        {
            HotKeyPressed -= (Action)d;
        }
    }

    private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        const int wmHotkey = 0x0312;

        switch (msg)
        {
            case wmHotkey:
                switch (wParam.ToInt32())
                {
                    case HotKeyId:
                        // Call event handler
                        HotKeyPressed();
                        handled = true;
                        break;
                }

                break;
        }

        return IntPtr.Zero;
    }
}
