using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using StoreCard.Commands;
using StoreCard.Models;
using StoreCard.Properties;
using StoreCard.Static;
using StoreCard.Utils;

namespace StoreCard.Windows;

/// <summary>
/// Interaction logic for SettingsWindow.xaml
/// </summary>
public partial class SettingsWindow : INotifyPropertyChanged
{
    private UserConfig _config;

    public SettingsWindow()
    {
        _config = AppData.ReadConfigFromFile();

        DataContext = this;
        InitializeComponent();

        RunOnStartupCheckBox.IsChecked = Shortcuts.IsStartupShortcutEnabled() != null;
        ThemeComboBox.SelectedItem = _config.Theme.ToString();
    }

    public static IEnumerable<string> Themes => Enum.GetValues(typeof(Theme))
        .Cast<Theme>()
        .Select(t => t.ToString());

    public string HotKeyText => HotKeys.KeyStringFromConfig(_config);

    public bool IsStartupShortcutDisabled => Shortcuts.IsStartupShortcutEnabled() == false;

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

    private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!Enum.TryParse((string) ThemeComboBox.SelectedItem, out Theme theme)) return;
        _config.Theme = theme;
        AppData.SaveConfigToFile(_config);
    }
}