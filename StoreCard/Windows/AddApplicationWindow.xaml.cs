﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using StoreCard.Commands;
using StoreCard.GameLibraries;
using StoreCard.Models.Items.Installed;
using StoreCard.Models.Items.Saved;
using StoreCard.Properties;
using StoreCard.UserControls;
using StoreCard.Utils;

namespace StoreCard.Windows;

/// <summary>
///     Interaction logic for AddApplicationWindow.xaml
/// </summary>
public partial class AddApplicationWindow : INotifyPropertyChanged
{
    public AddApplicationWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    public string ExecutableName
    {
        get => _executableName;
        set
        {
            _executableName = value;
            OnPropertyChanged(nameof(ExecutableName));
        }
    }

    public bool DoesExecutableExist
    {
        get => _doesExecutableExist;
        set
        {
            _doesExecutableExist = value;
            OnPropertyChanged(nameof(DoesExecutableExist));
        }
    }

    public ImageSource? ExecutableIcon
    {
        get => _executableIcon;
        set
        {
            _executableIcon = value;
            OnPropertyChanged(nameof(ExecutableIcon));
        }
    }

    public bool ShouldEnableSaveAppButton => AppListBox.SelectedItem != null;

    public bool ShouldEnableSaveGameButton => GameListBox.SelectedIndex != -1;

    public string? SelectedAppName => (AppListBox.SelectedItem as InstalledApplication)?.Name;

    public ImageSource? SelectedAppIcon => (AppListBox.SelectedItem as InstalledApplication)?.BitmapIcon;

    public string? SelectedGameName => (GameListBox.SelectedItem as InstalledGame)?.Name;

    public ImageSource? SelectedGameIcon => (GameListBox.SelectedItem as InstalledGame)?.BitmapIcon;

    private bool _doesExecutableExist;

    private ImageSource? _executableIcon;

    private string _executableName = "";

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void SaveAppButton_Click(object sender, RoutedEventArgs e)
    {
        SaveSelectedAppAndClose();
    }

    private void SaveGameButton_Click(object sender, RoutedEventArgs e)
    {
        SaveSelectedGameAndClose();
    }

    private void SaveSelectedAppAndClose()
    {
        var savedItems = AppData.ReadItemsFromFile();
        savedItems.Add(new SavedApplication((AppListBox.SelectedItem as InstalledApplication)!));
        AppData.SaveItemsToFile(savedItems);
        Close();
    }

    private void SaveSelectedGameAndClose()
    {
        var savedItems = AppData.ReadItemsFromFile();
        savedItems.Add((GameListBox.SelectedItem as InstalledGame)!.SavedItem);
        AppData.SaveItemsToFile(savedItems);
        Close();
    }

    private void LoadApps()
    {
        foreach (var app in Applications.GetInstalledApplications())
        {
            AppListBox.AddItem(app);
        }
        AppListBox.FinishAddingItems();
    }

    private void LoadGames()
    {
        foreach (var game in new SteamLibrary().GetInstalledGames()) {
            GameListBox.AddItem(game);
        }
        foreach (var game in new EpicLibrary().GetInstalledGames()) {
            GameListBox.AddItem(game);
        }
        GameListBox.FinishAddingItems();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        Activate();

        Task.Run(LoadApps);
        Task.Run(LoadGames);
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        new ShowMainWindowCommand().Execute();
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape) Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void AppListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(ShouldEnableSaveAppButton));
        OnPropertyChanged(nameof(SelectedAppName));
        OnPropertyChanged(nameof(SelectedAppIcon));
    }

    private void GameListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(ShouldEnableSaveGameButton));
        OnPropertyChanged(nameof(SelectedGameName));
        OnPropertyChanged(nameof(SelectedGameIcon));
    }

    private void AppListBox_ItemActivated(object sender, ItemActivatedEventArgs e)
    {
        SaveSelectedAppAndClose();
        e.Handled = true;
    }

    private void GameListBox_ItemActivated(object sender, ItemActivatedEventArgs e)
    {
        SaveSelectedGameAndClose();
        e.Handled = true;
    }

    private void ExecutableSelector_Finished(object sender, RoutedEventArgs e)
    {
        Close();
    }
}