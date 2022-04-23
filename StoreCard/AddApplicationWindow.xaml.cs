using System;
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
using SteamKit2;
using StoreCard.Annotations;

namespace StoreCard;

/// <summary>
///     Interaction logic for AddApplicationWindow.xaml
/// </summary>
public partial class AddApplicationWindow : INotifyPropertyChanged
{
    private bool _areGamesLoaded;

    private bool _doesExecutableExist;

    private ImageSource? _executableIcon;

    private string _executableName = "";

    private string _executablePath = "";

    private IEnumerable<InstalledGame> _filteredGames = new List<InstalledGame>();

    private string _gameSearchText = "";

    private List<InstalledGame> _installedGames = new();

    public AddApplicationWindow()
    {
        InitializeComponent();

        DataContext = this;
    }

    public IEnumerable<InstalledGame> FilteredGames
    {
        get => _filteredGames;
        set
        {
            _filteredGames = value;
            OnPropertyChanged(nameof(FilteredGames));
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (_filteredGames.Any()) GameListBox.SelectedIndex = 0;
            });
        }
    }

    public string GameSearchText
    {
        get => _gameSearchText;
        set
        {
            _gameSearchText = value;
            OnPropertyChanged(nameof(GameSearchText));
            FilteredGames = FilterGames();
        }
    }

    public string ExecutablePath
    {
        get => _executablePath;
        set
        {
            _executablePath = value;

            DoesExecutableExist = File.Exists(value);
            if (DoesExecutableExist)
            {
                // Take file name without '.exe'
                ExecutableName = value.Split(@"\").Last().Split(".")[0];

                var icon = System.Drawing.Icon.ExtractAssociatedIcon(value);
                if (icon != null)
                    ExecutableIcon = Imaging.CreateBitmapSourceFromHIcon(
                        icon.Handle,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
            }

            OnPropertyChanged(nameof(ExecutablePath));
        }
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

    public bool AreGamesLoaded
    {
        get => _areGamesLoaded;
        set
        {
            _areGamesLoaded = value;
            OnPropertyChanged(nameof(AreGamesLoaded));
        }
    }

    public bool ShouldEnableSaveAppButton => AppListBox.SelectedItem != null;

    public bool ShouldEnableSaveGameButton => GameListBox.SelectedIndex != -1;

    public string? SelectedAppName => (AppListBox.SelectedItem as InstalledApplication)?.Name;

    public ImageSource? SelectedAppIcon => (AppListBox.SelectedItem as InstalledApplication)?.BitmapIcon;

    public string? SelectedGameName => (GameListBox.SelectedItem as InstalledGame)?.Name;

    public ImageSource? SelectedGameIcon => (GameListBox.SelectedItem as InstalledGame)?.BitmapIcon;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void LoadInstalledSteamGames()
    {
        var installedGames = new List<InstalledSteamGame>();

        if (Paths.SteamInstallFolder == null) return;


        var libraryCacheFolder = $"{Paths.SteamInstallFolder}\\appcache\\librarycache";
        var steamAppsFolder = $"{Paths.SteamInstallFolder}\\steamapps";

        var libraryFolders = KeyValue.LoadFromString(File.ReadAllText($"{steamAppsFolder}\\libraryfolders.vdf"));
        if (libraryFolders == null) return;

        var steamAppsFolderPaths = libraryFolders.Children
            .Where(child => int.TryParse(child.Name, out _))
            .Select(kv => $"{kv["path"].Value}\\steamapps")
            .ToList();
        foreach (var appsFolderPath in steamAppsFolderPaths)
        {
            var appManifestPaths = Directory.GetFiles(appsFolderPath, "*.acf", SearchOption.AllDirectories);
            foreach (var manifestPath in appManifestPaths)
            {
                var manifest = KeyValue.LoadFromString(File.ReadAllText(manifestPath));
                if (manifest == null) continue;

                var name = manifest["name"].Value;
                var appId = manifest["appid"].Value;
                if (name == null || appId == null) continue;

                var iconPath = $"{libraryCacheFolder}\\{appId}_icon.jpg";
                if (!File.Exists(iconPath)) continue;

                Stream imageStreamSource = new FileStream(iconPath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read);
                var decoder = new JpegBitmapDecoder(imageStreamSource,
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.Default);
                BitmapSource bitmapIcon = decoder.Frames[0];

                // From https://stackoverflow.com/a/61681670
                var cached = new CachedBitmap(
                    bitmapIcon,
                    BitmapCreateOptions.None,
                    BitmapCacheOption.OnLoad);
                cached.Freeze();

                installedGames.Add(new InstalledSteamGame(name, appId, cached));
            }
        }

        SetInstalledGames(installedGames.Cast<InstalledGame>().ToList());
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void SetInstalledGames(List<InstalledGame> value)
    {
        _installedGames = value;
        _installedGames.Sort();
        AreGamesLoaded = true;
        FilteredGames = FilterGames();
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (_installedGames.Count > 0) GameListBox.SelectedIndex = 0;
        });
    }

    private void GameSearchBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Up:
                if (GameListBox.Items.Count == 0) return;
                switch (GameListBox.SelectedIndex)
                {
                    case 0:
                    case -1:
                        GameListBox.SelectedIndex = GameListBox.Items.Count - 1;
                        break;
                    default:
                        GameListBox.SelectedIndex = (GameListBox.SelectedIndex - 1) % GameListBox.Items.Count;
                        break;
                }

                GameListBox.ScrollIntoView(GameListBox.SelectedItem);
                break;
            case Key.Down:
                if (GameListBox.Items.Count == 0) return;
                if (GameListBox.SelectedIndex == -1)
                    GameListBox.SelectedIndex = 0;
                else
                    GameListBox.SelectedIndex = (GameListBox.SelectedIndex + 1) % GameListBox.Items.Count;
                GameListBox.ScrollIntoView(GameListBox.SelectedItem);
                break;
            case Key.Enter:
                AddSelectedGame();
                e.Handled = true;
                break;
        }
    }

    private void SaveAppButton_Click(object sender, RoutedEventArgs e)
    {
        SaveSelectedAppAndClose();
    }

    private void SaveGameButton_Click(object sender, RoutedEventArgs e)
    {
        AddSelectedGame();
    }

    private void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Executables|*.exe|All Files (*.*)|*.*",
            InitialDirectory = Environment.ExpandEnvironmentVariables("%ProgramW6432%"),
            Title = "Select Executable"
        };

        if (openFileDialog.ShowDialog() == true) ExecutablePath = openFileDialog.FileName;
    }

    private void SaveExecutableButton_Click(object sender, RoutedEventArgs e)
    {
        var base64Icon = ExecutableIcon != null ? ImageUtils.ImageToBase64((BitmapSource) ExecutableIcon) : null;
        var savedItems = StorageUtils.ReadItemsFromFile();
        savedItems.Add(new SavedExecutable(Guid.NewGuid().ToString(), ExecutableName, base64Icon, ExecutablePath));
        StorageUtils.SaveItemsToFile(savedItems);
        Close();
    }

    private void SaveSelectedAppAndClose()
    {
        var savedItems = StorageUtils.ReadItemsFromFile();
        savedItems.Add(new SavedApplication((AppListBox.SelectedItem as InstalledApplication)!));
        StorageUtils.SaveItemsToFile(savedItems);
        Close();
    }

    private void AddSelectedGame()
    {
        if (GameListBox.SelectedItem is not InstalledGame installedGame) return;
        var savedItems = StorageUtils.ReadItemsFromFile();
        savedItems.Add(installedGame.SavedItem);
        StorageUtils.SaveItemsToFile(savedItems);
        Close();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        Activate();

        Task.Run(() => AppListBox.Items = SystemUtils.GetInstalledApplications());
        Task.Run(LoadInstalledSteamGames);
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        new ShowMainWindowCommand().Execute(null);
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape) Close();
    }

    private IEnumerable<InstalledGame> FilterGames()
    {
        var games = _installedGames
            .Where(app => app.Name.ToUpper().StartsWith(_gameSearchText.ToUpper()));
        games = games.Concat(_installedGames.Where(game =>
        {
            return !game.Name.ToUpper().StartsWith(_gameSearchText.ToUpper()) &&
                   game.Name.ToUpper().Contains(_gameSearchText.ToUpper());
        }));
        return games;
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
}