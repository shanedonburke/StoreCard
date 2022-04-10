using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace StoreCard;

/// <summary>
///     Interaction logic for TaskbarIconWindow.xaml
/// </summary>
public partial class TaskbarIconWindow
{
    private const int HotkeyId = 9000;

    private HwndSource? _source;

    public TaskbarIconWindow()
    {
        InitializeComponent();

        TaskbarIcon.Icon = Properties.Resources.StoreCardIcon;

        DataContext = this;
    }

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

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        var helper = new WindowInteropHelper(this);
        _source = HwndSource.FromHwnd(helper.Handle);
        _source?.AddHook(HwndHook);
        RegisterHotKey();
    }

    protected override void OnClosed(EventArgs e)
    {
        _source?.RemoveHook(HwndHook);
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
        const uint vkX = 0x58;
        const uint modWinShift = 0x000C;
        if (!RegisterHotKey(helper.Handle, HotkeyId, modWinShift, vkX))
            Debug.WriteLine("Failed to register hotkey.");
    }

    private void UnregisterHotKey()
    {
        var helper = new WindowInteropHelper(this);
        UnregisterHotKey(helper.Handle, HotkeyId);
    }

    private IntPtr HwndHook(IntPtr hwnd,
        int msg,
        IntPtr wParam,
        IntPtr lParam,
        ref bool handled)
    {
        const int wmHotkey = 0x0312;
        switch (msg)
        {
            case wmHotkey:
                switch (wParam.ToInt32())
                {
                    case HotkeyId:
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

    private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
    {
        new ShowMainWindowCommand().Execute(null);
    }

    private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}