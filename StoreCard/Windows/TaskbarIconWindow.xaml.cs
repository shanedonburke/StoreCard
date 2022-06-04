#region

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Interop;
using StoreCard.Commands;
using StoreCard.Native;
using StoreCard.Properties;
using StoreCard.Services;

#endregion

namespace StoreCard.Windows;

/// <summary>
/// Invisible window for the taskbar icon. Handles hot key registration,
/// because some window must be continuously open for a hot key to be registered.
/// </summary>
public sealed partial class TaskbarIconWindow : INotifyPropertyChanged
{
    private string _hotKeyText = string.Empty;

    /// <summary>
    /// HWND of message windows.
    /// </summary>
    private const int HwndMessage = -3;

    public TaskbarIconWindow()
    {
        DataContext = this;
        InitializeComponent();
        TaskbarIcon.Icon = Properties.Resources.StoreCardIcon;

        // This makes sure that the window is never visible.
        // See https://stackoverflow.com/a/2575839
        User32.SetParent(new WindowInteropHelper(this).Handle, (IntPtr)HwndMessage);
    }

    /// <summary>
    /// Display string for the hot key
    /// </summary>
    public string HotKeyText
    {
        get => _hotKeyText;
        set
        {
            _hotKeyText = value;
            OnPropertyChanged(nameof(HotKeyText));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private static void OnHotKeyPressed() => new ShowSearchCommand().Execute();

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        HotKeyService.Instance.HotKeyRegistered += OnHotKeyRegistered;
        HotKeyService.Instance.RegisterHotKey(this, OnHotKeyPressed);
    }

    protected override void OnClosed(EventArgs e)
    {
        HotKeyService.Instance.UnregisterHotKey();
        base.OnClosed(e);
    }

    [NotifyPropertyChangedInvocator]
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void OnHotKeyRegistered(string hotKeyText) => HotKeyText = hotKeyText;

    private void OpenMenuItem_Click(object sender, RoutedEventArgs e) => new ShowSearchCommand().Execute();

    private void ExitMenuItem_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

    private void TaskbarIcon_TrayLeftMouseUp(object sender, RoutedEventArgs e) => new ShowSearchCommand().Execute();
}
