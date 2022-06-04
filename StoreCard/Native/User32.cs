#region

using System;
using System.Runtime.InteropServices;
using System.Text;

#endregion

namespace StoreCard.Native;

/// <summary>
/// Wrapper for <c>User32.dll</c>.
/// Used to interact with key and hot key APIs.
/// </summary>
public class User32
{
    [DllImport("User32.dll")]
    public static extern int ToAscii(
        uint uVirtKey,
        uint uScanCode,
        byte[] lpKeyState,
        [Out] StringBuilder lpChar,
        uint uFlags);

    // Hotkey solution from https://stackoverflow.com/a/11378213
    [DllImport("User32.dll")]
    public static extern bool RegisterHotKey(
        [In] IntPtr hWnd,
        [In] int id,
        [In] uint fsModifiers,
        [In] uint vk);

    [DllImport("User32.dll")]
    public static extern bool UnregisterHotKey(
        [In] IntPtr hWnd,
        [In] int id);

    [DllImport("user32.dll")]
    public static extern IntPtr SetParent(IntPtr hwnd, IntPtr hwndNewParent);
}
