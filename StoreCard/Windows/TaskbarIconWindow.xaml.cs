﻿using System;
using System.ComponentModel;
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
    private static void OnHotKeyPressed()
    {
        new ShowSearchCommand().Execute();
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

    private string _hotKeyText = string.Empty;

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
        new ShowSearchCommand().Execute();
    }

    private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void TaskbarIcon_TrayLeftMouseUp(object sender, RoutedEventArgs e)
    {
        new ShowSearchCommand().Execute();
    }
}
