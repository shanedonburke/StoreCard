using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using StoreCard.Commands;
using StoreCard.Properties;
using StoreCard.Services;

namespace StoreCard.Windows;

/// <summary>
///     Interaction logic for TaskbarIconWindow.xaml
/// </summary>
public partial class TaskbarIconWindow : INotifyPropertyChanged
{
    private static void OnHotKeyPressed() {
        // Don't open if windows other than this one and the main window are open
        if (Application.Current.Windows.Count > 2) {
            return;
        }
        new ShowMainWindowCommand().Execute();
    }

    public TaskbarIconWindow() {
        DataContext = this;
        InitializeComponent();
        TaskbarIcon.Icon = Properties.Resources.StoreCardIcon;
    }

    public string HotKeyText
    {
        get => _hotKeyText;
        set
        {
            _hotKeyText = value;
            OnPropertyChanged(nameof(HotKeyText));
        }
    }

    private string _hotKeyText = "";

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        HotKeyService.Instance.HotKeyRegistered += OnHotKeyRegistered;
        HotKeyService.Instance.RegisterHotKey(this, OnHotKeyPressed);
    }

    protected override void OnClosed(EventArgs e)
    {
        HotKeyService.Instance.UnregisterHotKey(OnHotKeyPressed);
        base.OnClosed(e);
    }

    private void OnHotKeyRegistered(string hotKeyText)
    {
        HotKeyText = hotKeyText;
    }

    private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
    {
        new ShowMainWindowCommand().Execute();
    }

    private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}