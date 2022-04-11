using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Shell;
using SteamKit2;

namespace StoreCard;

/// <summary>
///     Interaction logic for AddApplicationWindow.xaml
/// </summary>
public partial class AddApplicationWindow : INotifyPropertyChanged
{
    private string _appSearchText = "";

    private bool _areAppsLoaded;

    private bool _areGamesLoaded;

    private bool _doesExecutableExist;

    private ImageSource? _executableIcon;

    private string _executableName = "";

    private string _executablePath = "";

    private IEnumerable<InstalledApplication> _filteredApps = new List<InstalledApplication>();

    private IEnumerable<InstalledGame> _filteredGames = new List<InstalledGame>();

    private string _gameSearchText = "";

    private List<InstalledApplication> _installedApps = new();

    private List<InstalledGame> _installedGames = new();

    public AddApplicationWindow()
    {
        InitializeComponent();

        DataContext = this;
    }

    public IEnumerable<InstalledApplication> FilteredApps
    {
        get => _filteredApps;
        set
        {
            _filteredApps = value;
            OnPropertyChanged("FilteredApps");
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (_filteredApps.Any()) ApplicationListBox.SelectedIndex = 0;
            });
        }
    }

    public IEnumerable<InstalledGame> FilteredGames
    {
        get => _filteredGames;
        set
        {
            _filteredGames = value;
            OnPropertyChanged("FilteredGames");
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (_filteredGames.Any()) GameListBox.SelectedIndex = 0;
            });
        }
    }

    public string AppSearchText
    {
        get => _appSearchText;
        set
        {
            _appSearchText = value;
            OnPropertyChanged("AppSearchText");
            FilteredApps = FilterApps();
        }
    }

    public string GameSearchText
    {
        get => _gameSearchText;
        set
        {
            _gameSearchText = value;
            OnPropertyChanged("GameSearchText");
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

            OnPropertyChanged("ExecutablePath");
        }
    }

    public string ExecutableName
    {
        get => _executableName;
        set
        {
            _executableName = value;
            OnPropertyChanged("ExecutableName");
        }
    }

    public bool DoesExecutableExist
    {
        get => _doesExecutableExist;
        set
        {
            _doesExecutableExist = value;
            OnPropertyChanged("DoesExecutableExist");
        }
    }

    public ImageSource? ExecutableIcon
    {
        get => _executableIcon;
        set
        {
            _executableIcon = value;
            OnPropertyChanged("ExecutableIcon");
        }
    }

    public bool AreAppsLoaded
    {
        get => _areAppsLoaded;
        set
        {
            _areAppsLoaded = value;
            OnPropertyChanged("AreAppsLoaded");
        }
    }

    public bool AreGamesLoaded
    {
        get => _areGamesLoaded;
        set
        {
            _areGamesLoaded = value;
            OnPropertyChanged("AreGamesLoaded");
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    // From https://stackoverflow.com/a/57195200
    private void LoadInstalledApplications()
    {
        var installedApps = new List<InstalledApplication>();
        var appsFolderId = new Guid("{1e87508d-89c2-42f0-8a7e-645a0f50ca58}");
        var appsFolder = (ShellObject) KnownFolderHelper.FromKnownFolderId(appsFolderId);

        foreach (var app in (IKnownFolder) appsFolder)
        {
            // The friendly app name
            var name = app.Name;
            // The ParsingName property is the AppUserModelID
            var appUserModelId = app.ParsingName;
            var icon = app.Thumbnail.SmallBitmapSource;
            icon.Freeze();

            installedApps.Add(new InstalledApplication(name, appUserModelId, icon));
        }

        SetInstalledApps(installedApps);
    }

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

    private void OnPropertyChanged(string name)
    {
        if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name));
    }

    private void SetInstalledApps(List<InstalledApplication> value)
    {
        _installedApps = value;
        _installedApps.Sort();
        AreAppsLoaded = true;
        FilteredApps = FilterApps();
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (_installedApps.Count > 0) ApplicationListBox.SelectedIndex = 0;
        });
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

    private void AppSearchBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Up:
                if (ApplicationListBox.Items.Count == 0) return;
                switch (ApplicationListBox.SelectedIndex)
                {
                    case 0:
                    case -1:
                        ApplicationListBox.SelectedIndex = ApplicationListBox.Items.Count - 1;
                        break;
                    default:
                        ApplicationListBox.SelectedIndex =
                            (ApplicationListBox.SelectedIndex - 1) % ApplicationListBox.Items.Count;
                        break;
                }

                ApplicationListBox.ScrollIntoView(ApplicationListBox.SelectedItem);
                break;
            case Key.Down:
                if (ApplicationListBox.Items.Count == 0) return;
                if (ApplicationListBox.SelectedIndex == -1)
                    ApplicationListBox.SelectedIndex = 0;
                else
                    ApplicationListBox.SelectedIndex =
                        (ApplicationListBox.SelectedIndex + 1) % ApplicationListBox.Items.Count;
                ApplicationListBox.ScrollIntoView(ApplicationListBox.SelectedItem);
                break;
            case Key.Enter:
                AddSelectedApplication();
                e.Handled = true;
                break;
        }
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
        AddSelectedApplication();
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

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var base64Icon = ExecutableIcon != null ? ImageUtils.ImageToBase64((BitmapSource) ExecutableIcon) : null;
        var savedItems = StorageUtils.ReadItemsFromFile();
        savedItems.Add(new SavedExecutable(ExecutableName, base64Icon, ExecutablePath));
        StorageUtils.SaveItemsToFile(savedItems);
        Close();
    }

    private void ApplicationListBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        AddSelectedApplication();
        e.Handled = true;
    }

    private void AddSelectedApplication()
    {
        if (ApplicationListBox.SelectedItem is not InstalledApplication installedApplication) return;
        var savedItems = StorageUtils.ReadItemsFromFile();
        savedItems.Add(new SavedApplication(installedApplication));
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

        Task.Run(LoadInstalledApplications);
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

    private IEnumerable<InstalledApplication> FilterApps()
    {
        var apps = _installedApps
            .Where(app => app.Name.ToUpper().StartsWith(_appSearchText.ToUpper()));
        apps = apps.Concat(_installedApps.Where(app =>
        {
            return !app.Name.ToUpper().StartsWith(_appSearchText.ToUpper()) &&
                   app.Name.ToUpper().Contains(_appSearchText.ToUpper());
        }));
        return apps;
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
}