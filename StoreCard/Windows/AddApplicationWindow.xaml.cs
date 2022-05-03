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
using Newtonsoft.Json;
using SteamKit2;
using StoreCard.Commands;
using StoreCard.Models.Games.Epic;
using StoreCard.Models.Items.Installed;
using StoreCard.Models.Items.Saved;
using StoreCard.Properties;
using StoreCard.Static;
using StoreCard.UserControls;
using StoreCard.Utils;

namespace StoreCard.Windows;

/// <summary>
///     Interaction logic for AddApplicationWindow.xaml
/// </summary>
public partial class AddApplicationWindow : INotifyPropertyChanged
{
    private bool _doesExecutableExist;

    private ImageSource? _executableIcon;

    private string _executableName = "";

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

    public event PropertyChangedEventHandler? PropertyChanged;

    private IEnumerable<InstalledGame> GetInstalledGames()
    {
        return GetInstalledSteamGames().Concat(GetInstalledEpicGames());
    }

    private IEnumerable<InstalledGame> GetInstalledSteamGames()
    {
        var installedGames = new List<InstalledGame>();

        if (Paths.SteamInstallFolder == null) return installedGames;

        var libraryCacheFolder = $"{Paths.SteamInstallFolder}\\appcache\\librarycache";
        var steamAppsFolder = $"{Paths.SteamInstallFolder}\\steamapps";

        var libraryFolders = KeyValue.LoadFromString(File.ReadAllText($"{steamAppsFolder}\\libraryfolders.vdf"));
        if (libraryFolders == null) return installedGames;

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

                installedGames.Add(new InstalledSteamGame(name, cached, appId));
            }
        }
        installedGames.Sort();
        return installedGames;
    }

    private IEnumerable<InstalledGame> GetInstalledEpicGames()
    {
        var installedGames = new List<InstalledGame>();

        var programDataFolder = Environment.ExpandEnvironmentVariables("%ProgramData%");

        var launcherInstalledPath = Path.Combine(programDataFolder,
            @"Epic\UnrealEngineLauncher\LauncherInstalled.dat");
        var manifestFolderPath = Path.Combine(programDataFolder, @"Epic\EpicGamesLauncher\Data\Manifests");

        if (!File.Exists(launcherInstalledPath) || !Directory.Exists(manifestFolderPath))
        {
            return installedGames;
        }

        if (JsonConvert.DeserializeObject<EpicLauncherInstalled>(File.ReadAllText(launcherInstalledPath)) is not { } launcherInstalled)
        {
            return installedGames;
        }

        var appNames = launcherInstalled.InstallationList.Select(app => app.AppName).ToList();

        var manifestPaths = Directory.GetFiles(manifestFolderPath, "*.item");

        foreach (var manifestPath in manifestPaths)
        {
            if (JsonConvert.DeserializeObject<EpicManifest>(File.ReadAllText(manifestPath)) is not { } manifest)
            {
                return installedGames;
            }

            if (!appNames.Contains(manifest.AppName)) continue;

            var execPath = Path.Combine(manifest.InstallLocation, manifest.LaunchExecutable);
            if (!File.Exists(execPath)) continue;

            var hIcon = System.Drawing.Icon.ExtractAssociatedIcon(execPath);
            var icon = Imaging.CreateBitmapSourceFromHIcon(
                hIcon!.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            icon.Freeze();

            installedGames.Add(new InstalledEpicGame(manifest.DisplayName, icon, manifest.AppName));
        }

        return installedGames;
    }
    
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

    private void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Executables|*.exe|All Files (*.*)|*.*",
            InitialDirectory = Environment.ExpandEnvironmentVariables("%ProgramW6432%"),
            Title = "Select Executable"
        };

        if (openFileDialog.ShowDialog() == true) ExecutablePathBox.Text = openFileDialog.FileName;
    }

    private void SaveExecutableButton_Click(object sender, RoutedEventArgs e)
    {
        var base64Icon = ExecutableIcon != null ? ImageUtils.ImageToBase64((BitmapSource) ExecutableIcon) : null;
        var savedItems = StorageUtils.ReadItemsFromFile();
        savedItems.Add(new SavedExecutable(Guid.NewGuid().ToString(), ExecutableName, base64Icon, ExecutablePathBox.Text));
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

    private void SaveSelectedGameAndClose()
    {
        var savedItems = StorageUtils.ReadItemsFromFile();
        savedItems.Add((GameListBox.SelectedItem as InstalledGame)!.SavedItem);
        StorageUtils.SaveItemsToFile(savedItems);
        Close();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        Activate();

        Task.Run(() => AppListBox.Items = SystemUtils.GetInstalledApplications());
        Task.Run(() => GameListBox.Items = GetInstalledGames());
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        new ShowMainWindowCommand().Execute(null);
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

    private void ExecutablePathBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var text = ExecutablePathBox.Text;
        DoesExecutableExist = File.Exists(text);
        if (!DoesExecutableExist) return;
        // Take file name without '.exe'
        ExecutableName = text.Split(@"\").Last().Split(".")[0];

        var icon = System.Drawing.Icon.ExtractAssociatedIcon(text);
        if (icon != null)
            ExecutableIcon = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
    }

    private void ExecutableNameBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        ExecutableName = ExecutableNameBox.Text;
    }
}