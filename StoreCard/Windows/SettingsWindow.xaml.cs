using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using StoreCard.Commands;
using StoreCard.Models;
using StoreCard.Properties;
using StoreCard.Services;
using StoreCard.Utils;

namespace StoreCard.Windows;

/// <summary>
/// Interaction logic for SettingsWindow.xaml
/// </summary>
public partial class SettingsWindow : INotifyPropertyChanged
{

    private UserConfig _config;

    public string HotKeyText => HotKeys.KeyStringFromConfig(_config);

    public bool IsStartupShortcutDisabled => Shortcuts.IsStartupShortcutEnabled() == false;

    public SettingsWindow() {
        InitializeComponent();
        _config = AppData.ReadConfigFromFile();
        DataContext = this;

        RunOnStartupCheckBox.IsChecked = Shortcuts.IsStartupShortcutEnabled() != null;
    }

    private void SettingsWindow_Closed(object? sender, EventArgs e)
    {
        AppData.SaveConfigToFile(_config);
        new ShowMainWindowCommand().Execute();
        Close();
    }

    private void RecordHotKeyButton_Click(object sender, RoutedEventArgs e)
    {
        if (new RecordHotKeyWindow().ShowDialog() != true) return;
        _config = AppData.ReadConfigFromFile();
        OnPropertyChanged(nameof(HotKeyText));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void RunOnStartupCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        Shortcuts.CreateStartupShortcut();
        OnPropertyChanged(nameof(IsStartupShortcutDisabled));
    }

    private void RunOnStartupCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        Shortcuts.RemoveStartupShortcut();
        OnPropertyChanged(nameof(IsStartupShortcutDisabled));
    }

    private void UseDefaultHotKeyButton_Click(object sender, RoutedEventArgs e)
    {
        _config.ResetHotKeyToDefault();
        AppData.SaveConfigToFile(_config);
        OnPropertyChanged(nameof(HotKeyText));
    }
}