using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Interop;
using StoreCard.Commands;
using StoreCard.Properties;
using StoreCard.Utils;

namespace StoreCard.Windows;

/// <summary>
///     Interaction logic for TaskbarIconWindow.xaml
/// </summary>
public partial class TaskbarIconWindow : INotifyPropertyChanged
{
    private HwndSource? _source;

    private string _hotKeyText = "";

    public string HotKeyText
    {
        get => _hotKeyText;
        set
        {
            _hotKeyText = value;
            OnPropertyChanged(nameof(HotKeyText));
        }
    }

    public TaskbarIconWindow()
    {
        DataContext = this;

        InitializeComponent();

        TaskbarIcon.Icon = Properties.Resources.StoreCardIcon;
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        HotKeyService.Instance.RegisterHotKey(this, OnHotKeyPressed);
    }

    protected override void OnClosed(EventArgs e)
    {
        HotKeyService.Instance.UnregisterHotKey(OnHotKeyPressed);
        base.OnClosed(e);
    }

    private void OnHotKeyPressed()
    {
        if (Application.Current.Windows.Cast<Window>().Any(w => w is RecordHotKeyWindow))
        {
            return;
        }
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

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}