using System;
using System.Runtime.InteropServices;
using System.Windows;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Interop;

namespace StoreCard.Utils
{
    public static class WindowHelper
    {
        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        [DllImport("kernel32.dll")]
        static extern uint GetCurrentThreadId();

        [DllImport("user32.dll")]
        static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hWnd, uint Msg);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        private static void ForceForegroundWindow(IntPtr hwnd)
        {
            uint windowThreadProcessId = GetWindowThreadProcessId(GetForegroundWindow(), IntPtr.Zero);
            uint currentThreadId = GetCurrentThreadId();
            uint CONST_SW_SHOW = 5;
            AttachThreadInput(windowThreadProcessId, currentThreadId, true);
            BringWindowToTop(hwnd);
            ShowWindow(hwnd, CONST_SW_SHOW);
            AttachThreadInput(windowThreadProcessId, currentThreadId, false);
        }

        public static void ShowForeground(this System.Windows.Window window)
        {
            window.Show();
            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            ForceForegroundWindow(hwnd);
        }
    }
}
