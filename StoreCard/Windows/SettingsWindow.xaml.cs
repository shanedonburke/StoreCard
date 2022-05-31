#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StoreCard.Commands;
using StoreCard.Models;
using StoreCard.Properties;
using StoreCard.Utils;

#endregion

namespace StoreCard.Windows;

/// <summary>
/// A window that allows the user to configure StoreCard.
/// </summary>
public sealed partial class SettingsWindow : INotifyPropertyChanged
{
    private UserConfig _userConfig;

    public SettingsWindow()
    {
        _userConfig = AppData.ReadConfigFromFile();
        DataContext = this;
        InitializeComponent();

        // Check box if the startup shortcut file exists (may be disabled in Task Manager)
        RunOnStartupCheckBox.IsChecked = ShortcutUtils.IsStartupShortcutEnabled() != null;

        ShowPrefixIconsCheckBox.IsChecked = _userConfig.ShouldShowPrefixIcons;
        ThemeComboBox.SelectedItem = _userConfig.Theme;
    }

    /// <summary>
    /// Known theme names.
    /// </summary>
    public static IEnumerable<string> Themes => ThemeFinder.FindThemes();

    /// <summary>
    /// Display string for the hot key.
    /// </summary>
    public string HotKeyText => HotKeyUtils.KeyStringFromConfig(_userConfig);

    /// <summary>
    /// Whether the startup shortcut exists but is disabled.
    /// </summary>
    public bool IsStartupShortcutDisabled => ShortcutUtils.IsStartupShortcutEnabled() == false;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void SettingsWindow_Closed(object? sender, EventArgs e)
    {
        AppData.SaveConfigToFile(_userConfig);
        new ShowSearchCommand().Execute();
        Close();
    }

    private void RecordHotKeyButton_Click(object sender, RoutedEventArgs e)
    {
        if (new RecordHotKeyWindow().ShowDialog() != true)
        {
            return;
        }

        // Refresh config with new hot key if one was set
        _userConfig = AppData.ReadConfigFromFile();
        OnPropertyChanged(nameof(HotKeyText));
    }

    [NotifyPropertyChangedInvocator]
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void RunOnStartupCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        ShortcutUtils.CreateStartupShortcut();
        OnPropertyChanged(nameof(IsStartupShortcutDisabled));
    }

    private void RunOnStartupCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        ShortcutUtils.RemoveStartupShortcut();
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

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        // Close the window if Escape is pressed
        if (e.Key == Key.Escape)
        {
            Close();
        }
    }
}
