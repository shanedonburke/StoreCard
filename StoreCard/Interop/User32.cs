using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace StoreCard.Interop
{
    internal class User32
    {
        [DllImport("User32.dll")]
        public static extern int ToAscii(uint uVirtKey, uint uScanCode,
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
}
