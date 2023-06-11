#region

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows;
using StoreCard.Commands;
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
    private bool _justPressedHotkey;

    public TaskbarIconWindow()
    {
        DataContext = this;
        InitializeComponent();
        TaskbarIcon.Icon = Properties.Resources.StoreCardIcon;
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

    // private static void OnHotKeyPressed() => new ShowSearchCommand().Execute();
    private void OnHotKeyPressed()
    {
        if (_justPressedHotkey)
        {
            _justPressedHotkey = false;
            new ShowSearchCommand().Execute();
        }
        else
        {
            _justPressedHotkey = true;

            var timer = new Timer();
            timer.Interval = 300;
            timer.AutoReset = false;
            timer.Elapsed += (_, _) => _justPressedHotkey = false;
            timer.Start();
        }
    }

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

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        Hide();
    }
}
