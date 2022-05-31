#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using StoreCard.Commands;
using StoreCard.GameLibraries.Ea;
using StoreCard.GameLibraries.Epic;
using StoreCard.GameLibraries.Itch;
using StoreCard.GameLibraries.Steam;
using StoreCard.Models.Items.Installed;
using StoreCard.Models.Items.Installed.Games;
using StoreCard.Models.Items.Saved;
using StoreCard.Properties;
using StoreCard.UserControls;
using StoreCard.Utils;

#endregion

namespace StoreCard.Windows;

/// <summary>
/// A window that allows the user to select and save a new app, game, or executable (as an app).
/// </summary>
public sealed partial class AddAppWindow : INotifyPropertyChanged
{
    private bool _doesExecutableExist;

    private ImageSource? _executableIcon;

    private string _executableName = string.Empty;

    public AddAppWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    /// <summary>
    /// For executables, the display name of the new executable.
    /// </summary>
    public string ExecutableName
    {
        get => _executableName;
        set
        {
            _executableName = value;
            OnPropertyChanged(nameof(ExecutableName));
        }
    }

    /// <summary>
    /// For executables, whether the current path refers to a real file.
    /// </summary>
    public bool DoesExecutableExist
    {
        get => _doesExecutableExist;
        set
        {
            _doesExecutableExist = value;
            OnPropertyChanged(nameof(DoesExecutableExist));
        }
    }

    /// <summary>
    /// For executables, the icon derived from the executable file.
    /// </summary>
    public ImageSource? ExecutableIcon
    {
        get => _executableIcon;
        set
        {
            _executableIcon = value;
            OnPropertyChanged(nameof(ExecutableIcon));
        }
    }

    /// <summary>
    /// For games, enable the Save button if a selection has been made.
    /// </summary>
    public bool ShouldEnableSaveGameButton => GameListBox.SelectedIndex != -1;

    /// <summary>
    /// For games, the name of the currently selected game.
    /// </summary>
    public string? SelectedGameName => GameListBox.SelectedItem?.Name;

    /// <summary>
    /// For games, the icon for the currently selected game.
    /// </summary>
    public ImageSource? SelectedGameIcon => GameListBox.SelectedItem?.BitmapIcon;

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    /// <summary>
    /// For apps, save the selected app and close the window.
    /// </summary>
    private void SaveSelectedAppAndClose()
    {
        if (AppSelector.SelectedApp == null)
        {
            Logger.Log("Tried to save app, but no app was selected.");
        }
        else
        {
            List<SavedItem> savedItems = AppData.ReadItemsFromFile();
            savedItems.Add(AppSelector.SelectedApp!.SavedItem);
            AppData.SaveItemsToFile(savedItems);
        }

        Close();
    }

    /// <summary>
    /// For games, save the selected game and close the window.
    /// </summary>
    private void SaveSelectedGameAndClose()
    {
        List<SavedItem> savedItems = AppData.ReadItemsFromFile();
        savedItems.Add((GameListBox.SelectedItem as IInstalledItem)!.SavedItem);
        AppData.SaveItemsToFile(savedItems);
        Close();
    }

    /// <summary>
    /// Load the list of installed games into the game list box.
    /// </summary>
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

        foreach (InstalledGame game in new ItchLibrary().GetInstalledGames())
        {
            GameListBox.AddItem(game);
        }

        // Battle.net games are detected as apps
        foreach (InstalledApp game in InstalledAppUtils.GetBattleNetGames())
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

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        // Close the window if Escape is pressed
        if (e.Key == Key.Escape)
        {
            Close();
        }
    }

    private void Window_Closed(object sender, EventArgs e) => new ShowSearchCommand().Execute();

    private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();

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

    private void SaveGameButton_Click(object sender, RoutedEventArgs e) => SaveSelectedGameAndClose();

    private void ExecutableSelector_Finished(object sender, RoutedEventArgs e) => Close();

    private void AppSelector_SaveButtonClick(object sender, RoutedEventArgs e) => SaveSelectedAppAndClose();

    private void AppSelector_CancelButtonClick(object sender, RoutedEventArgs e) => Close();
}
