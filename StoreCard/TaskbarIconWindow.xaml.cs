using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StoreCard
{
    /// <summary>
    /// Interaction logic for TaskbarIconWindow.xaml
    /// </summary>
    public partial class TaskbarIconWindow : Window
    {
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

        private HwndSource _source;
        private const int HOTKEY_ID = 9000;

        public TaskbarIconWindow()
        {
            InitializeComponent();

            DataContext = this;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var helper = new WindowInteropHelper(this);
            _source = HwndSource.FromHwnd(helper.Handle);
            _source.AddHook(HwndHook);
            RegisterHotKey();
        }

        protected override void OnClosed(EventArgs e)
        {
            _source.RemoveHook(HwndHook);
            _source = null;
            UnregisterHotKey();
            base.OnClosed(e);
        }

        // See https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-registerhotkey
        // and https://docs.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes
        // for key codes
        private void RegisterHotKey()
        {
            var helper = new WindowInteropHelper(this);
            const uint VK_X = 0x58;
            const uint MOD_WIN_SHIFT = 0x000C;
            if (!RegisterHotKey(helper.Handle, HOTKEY_ID, MOD_WIN_SHIFT, VK_X))
            {
                System.Diagnostics.Debug.WriteLine("Failed to register hotkey.");
            }
        }

        private void UnregisterHotKey()
        {
            var helper = new WindowInteropHelper(this);
            UnregisterHotKey(helper.Handle, HOTKEY_ID);
        }

        private IntPtr HwndHook(IntPtr hwnd,
                                int msg,
                                IntPtr wParam,
                                IntPtr lParam,
                                ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_ID:
                            OnHotKeyPressed();
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        private void OnHotKeyPressed()
        {
            new ShowMainWindowCommand().Execute(null);
        }
    }
}
