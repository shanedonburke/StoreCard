#region

using System;
using System.Runtime.InteropServices;
using System.Text;

#endregion

namespace StoreCard.Native;

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
}
