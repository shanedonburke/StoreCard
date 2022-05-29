﻿#region

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using StoreCard.Models;
using StoreCard.Properties;
using StoreCard.Utils;

#endregion

namespace StoreCard.Windows;

/// <summary>
/// Interaction logic for RecordHotKeyWindow.xaml
/// </summary>
public partial class RecordHotKeyWindow : INotifyPropertyChanged
{
    private readonly UserConfig _config;
    private string _hotKeyText = string.Empty;
    private uint _modifiers;
    private uint _virtualKey;

    public RecordHotKeyWindow()
    {
        InitializeComponent();
        _config = AppData.ReadConfigFromFile();
        HotKeyText = HotKeyUtils.KeyStringFromConfig(_config);
        DataContext = this;
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

    public event PropertyChangedEventHandler? PropertyChanged;

    private void RecordHotKeyWindow_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        Key key = e.Key == Key.System ? e.SystemKey : e.Key;

        // Ignore modifier keys.
        if (key is Key.LeftShift or Key.RightShift or Key.LeftCtrl or Key.RightCtrl or Key.LeftAlt or Key.RightAlt
            or Key.LWin or Key.RWin)
        {
            return;
        }

        _modifiers = 0;
        _virtualKey = 0;

        StringBuilder text = new();
        if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
        {
            text.Append("Ctrl+");
            _modifiers |= (uint)ModifierKeys.Control;
        }

        if ((Keyboard.Modifiers & ModifierKeys.Alt) != 0)
        {
            text.Append("Alt+");
            _modifiers |= (uint)ModifierKeys.Alt;
        }

        if ((Keyboard.Modifiers & ModifierKeys.Windows) != 0 || Keyboard.IsKeyDown(Key.LWin) ||
            Keyboard.IsKeyDown(Key.RWin))
        {
            text.Append("Win+");
            _modifiers |= (uint)ModifierKeys.Windows;
        }

        if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
        {
            text.Append("Shift+");
            _modifiers |= (uint)ModifierKeys.Shift;
        }

        // Ignore tab press with no modifiers (to navigate UI)
        if (_modifiers == 0 && key is Key.Tab)
        {
            return;
        }

        e.Handled = true;

        _virtualKey = HotKeyUtils.KeyToVirtualKey(key);
        text.Append(HotKeyUtils.KeyToString(key));
        HotKeyText = text.ToString();
    }

    [NotifyPropertyChangedInvocator]
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void ClearButton_Click(object sender, RoutedEventArgs e) =>
        HotKeyText = HotKeyUtils.KeyStringFromConfig(_config);

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        _config.HotKeyModifiers = _modifiers;
        _config.VirtualHotKey = _virtualKey;
        AppData.SaveConfigToFile(_config);
        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
