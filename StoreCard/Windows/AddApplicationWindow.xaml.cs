using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using StoreCard.GameLibraries.BattleNet;
using StoreCard.GameLibraries.Ea;
using StoreCard.GameLibraries.Epic;
using StoreCard.GameLibraries.Itch;
using StoreCard.GameLibraries.Steam;
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

    public bool ShouldEnableSaveGameButton => GameListBox.SelectedIndex != -1;

    public string? SelectedGameName => (GameListBox.SelectedItem as InstalledGame)?.Name;

    public ImageSource? SelectedGameIcon => (GameListBox.SelectedItem as InstalledGame)?.BitmapIcon;

    private bool _doesExecutableExist;

    private ImageSource? _executableIcon;

    private string _executableName = "";

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void SaveGameButton_Click(object sender, RoutedEventArgs e)
    {
        SaveSelectedGameAndClose();
    }

    private void SaveSelectedAppAndClose()
    {
        if (AppSelector.SelectedApp == null)
        {
            Debug.WriteLine("Tried to save app, but no app was selected.");
        }
        else
        {
            List<SavedItem> savedItems = AppData.ReadItemsFromFile();
            savedItems.Add(new SavedApplication(AppSelector.SelectedApp!));
            AppData.SaveItemsToFile(savedItems);
        }

        Close();
    }

    private void SaveSelectedGameAndClose()
    {
        List<SavedItem> savedItems = AppData.ReadItemsFromFile();
        savedItems.Add((GameListBox.SelectedItem as InstalledGame)!.SavedItem);
        AppData.SaveItemsToFile(savedItems);
        Close();
    }

    private void LoadGames()
    {
        foreach (InstalledGame game in new SteamLibrary().GetInstalledGames())
        {
            GameListBox.AddItem(game);
        }

        foreach (InstalledGame game in new EpicLibrary().GetInstalledGames())
        {
            GameListBox.AddItem(game);
        }

        foreach (InstalledGame game in new EaLibrary().GetInstalledGames())
        {
            GameListBox.AddItem(game);
        }

        foreach (InstalledGame game in new BattleNetLibrary().GetInstalledGames())
        {
            GameListBox.AddItem(game);
        }

        foreach (InstalledGame game in new ItchLibrary().GetInstalledGames())
        {
            GameListBox.AddItem(game);
        }

        GameListBox.FinishAddingItems();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        Activate();
        Task.Run(LoadGames);
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        new ShowSearchCommand().Execute();
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape) Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void GameListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(ShouldEnableSaveGameButton));
        OnPropertyChanged(nameof(SelectedGameName));
        OnPropertyChanged(nameof(SelectedGameIcon));
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

    private void AppSelector_SaveButtonClick(object sender, RoutedEventArgs e)
    {
        SaveSelectedAppAndClose();
    }

    private void AppSelector_CancelButtonClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
