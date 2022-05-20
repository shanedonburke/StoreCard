using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using StoreCard.Commands;
using StoreCard.Models;
using StoreCard.Properties;
using StoreCard.Utils;

namespace StoreCard.Windows;

/// <summary>
/// Interaction logic for SettingsWindow.xaml
/// </summary>
public partial class SettingsWindow : INotifyPropertyChanged
{
    private UserConfig _userConfig;

    public SettingsWindow()
    {
        _userConfig = AppData.ReadConfigFromFile();

        DataContext = this;
        InitializeComponent();

        RunOnStartupCheckBox.IsChecked = Shortcuts.IsStartupShortcutEnabled() != null;
        ShowPrefixIconsCheckBox.IsChecked = _userConfig.ShouldShowPrefixIcons;
        ThemeComboBox.SelectedItem = _userConfig.Theme;
    }

    public static IEnumerable<string> Themes => ThemeFinder.FindThemes();

    public string HotKeyText => HotKeys.KeyStringFromConfig(_userConfig);

    public bool IsStartupShortcutDisabled => Shortcuts.IsStartupShortcutEnabled() == false;

    private void SettingsWindow_Closed(object? sender, EventArgs e)
    {
        AppData.SaveConfigToFile(_userConfig);
        new ShowMainWindowCommand().Execute();
        Close();
    }

    private void RecordHotKeyButton_Click(object sender, RoutedEventArgs e)
    {
        if (new RecordHotKeyWindow().ShowDialog() != true) return;
        _userConfig = AppData.ReadConfigFromFile();
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
        _userConfig.ResetHotKeyToDefault();
        AppData.SaveConfigToFile(_userConfig);
        OnPropertyChanged(nameof(HotKeyText));
    }

    private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _userConfig.Theme = (string)ThemeComboBox.SelectedItem;
        AppData.SaveConfigToFile(_userConfig);
    }

    private void ShowPrefixIconsCheckBox_Changed(object sender, RoutedEventArgs e)
    {
        _userConfig.ShouldShowPrefixIcons = ShowPrefixIconsCheckBox.IsChecked == true;
        AppData.SaveConfigToFile(_userConfig);
    }
}
