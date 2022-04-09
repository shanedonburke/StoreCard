﻿using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Shell;
using Newtonsoft.Json;
using SteamKit2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StoreCard
{
    /// <summary>
    /// Interaction logic for AddApplicationWindow.xaml
    /// </summary>
    public partial class AddApplicationWindow : Window, INotifyPropertyChanged
    {
        public IEnumerable<InstalledGame> InstalledGames
        {
            get => _installedGames;
            set
            {
                _installedGames = value.ToList();
                _installedGames.Sort();
                AreGamesLoaded = true;
                OnPropertyChanged("InstalledGames");
                GameListBox.SelectedIndex = 0;
            }
        }

        public IEnumerable<InstalledApplication> FilteredApps
        {
            get
            {
                IEnumerable<InstalledApplication> apps = _installedApps
                    .Where(app => app.Name.ToUpper().StartsWith(_appSearchText.ToUpper()));
                apps = apps.Concat(_installedApps.Where(app =>
                {
                    return !app.Name.ToUpper().StartsWith(_appSearchText.ToUpper()) &&
                        app.Name.ToUpper().Contains(_appSearchText.ToUpper());
                }));
                return apps;
            }
        }

        public string AppSearchText
        {
            get => _appSearchText;
            set
            {
                _appSearchText = value;
                OnPropertyChanged("SearchText");
                OnPropertyChanged("FilteredApps");
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

                    Icon? icon = System.Drawing.Icon.ExtractAssociatedIcon(value);
                    if (icon != null)
                    {
                        ExecutableIcon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                            icon.Handle,
                            Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions());
                    }
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

        private List<InstalledApplication> _installedApps = new List<InstalledApplication>();

        private List<InstalledGame> _installedGames = new List<InstalledGame>();

        private string _appSearchText = "";

        private string _executablePath = "";

        private string _executableName = "";

        private bool _doesExecutableExist = false;

        private ImageSource? _executableIcon = null;

        private bool _areAppsLoaded = false;

        private bool _areGamesLoaded = false;

        public AddApplicationWindow()
        {
            InitializeComponent();

            DataContext = this;
        }

        // From https://stackoverflow.com/a/57195200
        List<InstalledApplication> GetInstalledApplications()
        {
            var installedApps = new List<InstalledApplication>();
            var appsFolderId = new Guid("{1e87508d-89c2-42f0-8a7e-645a0f50ca58}");
            ShellObject appsFolder = (ShellObject)KnownFolderHelper.FromKnownFolderId(appsFolderId);

            foreach (var app in (IKnownFolder)appsFolder)
            {
                // The friendly app name
                string name = app.Name;
                // The ParsingName property is the AppUserModelID
                string appUserModelId = app.ParsingName;
                BitmapSource icon = app.Thumbnail.ExtraLargeBitmapSource;
                icon.Freeze();

                installedApps.Add(new InstalledApplication(name, appUserModelId, icon));
            }
            return installedApps;
        }

        List<InstalledSteamGame> GetInstalledSteamGames()
        {
            List<InstalledSteamGame> installedGames = new List<InstalledSteamGame>();

            if (Paths.SteamInstallFolder == null) return installedGames;


            string libraryCacheFolder = $"{Paths.SteamInstallFolder}\\appcache\\librarycache";
            string steamAppsFolder = $"{Paths.SteamInstallFolder}\\steamapps";

            KeyValue? libraryFolders = KeyValue.LoadFromString(File.ReadAllText($"{steamAppsFolder}\\libraryfolders.vdf"));
            if (libraryFolders == null) return installedGames;

            List<string> steamAppsFolderPaths = libraryFolders.Children
                    .Where(child => int.TryParse(child.Name, out int i))
                    .Select(kv => $"{kv["path"].Value}\\steamapps")
                    .ToList();
            foreach (string appsFolderPath in steamAppsFolderPaths)
            {
                string[] appManifestPaths = Directory.GetFiles(appsFolderPath, "*.acf", SearchOption.AllDirectories);
                foreach (string manifestPath in appManifestPaths)
                {
                    KeyValue? manifest = KeyValue.LoadFromString(File.ReadAllText(manifestPath));
                    if (manifest == null) continue;

                    string? name = manifest["name"].Value;
                    string? appId = manifest["appid"].Value;
                    if (name == null || appId == null) continue;

                    Stream imageStreamSource = new FileStream($"{libraryCacheFolder}\\{appId}_icon.jpg",
                                                              FileMode.Open,
                                                              FileAccess.Read,
                                                              FileShare.Read);
                    JpegBitmapDecoder decoder = new JpegBitmapDecoder(imageStreamSource,
                                                                      BitmapCreateOptions.PreservePixelFormat,
                                                                      BitmapCacheOption.Default);
                    BitmapSource bitmapIcon = decoder.Frames[0];

                    // From https://stackoverflow.com/a/61681670
                    CachedBitmap cached = new CachedBitmap(
                        bitmapIcon,
                        BitmapCreateOptions.None,
                        BitmapCacheOption.OnLoad);
                    cached.Freeze();

                    installedGames.Add(new InstalledSteamGame(name, appId, cached));
                }
            }
            return installedGames;
        }

        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name));
            if (ApplicationListBox != null && ApplicationListBox.Items.Count > 0)
            {
                ApplicationListBox.SelectedIndex = 0;
            }
        }

        private void SetInstalledApps(List<InstalledApplication> value)
        {
            _installedApps = value;
            _installedApps.Sort();
            AreAppsLoaded = true;
            OnPropertyChanged("FilteredApps");
            if (_installedApps.Count > 0)
            {
                ApplicationListBox.SelectedIndex = 0;
            }
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
                            ApplicationListBox.SelectedIndex = (ApplicationListBox.SelectedIndex - 1) % ApplicationListBox.Items.Count;
                            break;
                    }
                    ApplicationListBox.ScrollIntoView(ApplicationListBox.SelectedItem);
                    break;
                case Key.Down:
                    if (ApplicationListBox.Items.Count == 0) return;
                    if (ApplicationListBox.SelectedIndex == -1)
                    {
                        ApplicationListBox.SelectedIndex = 0;
                    }
                    else
                    {
                        ApplicationListBox.SelectedIndex = (ApplicationListBox.SelectedIndex + 1) % ApplicationListBox.Items.Count;
                    }
                    ApplicationListBox.ScrollIntoView(ApplicationListBox.SelectedItem);
                    break;
                case Key.Enter:
                    AddSelectedApplication();
                    e.Handled = true;
                    break;
                default:
                    break;
            }
        }

        private void AddAppButton_Click(object sender, RoutedEventArgs e)
        {
            AddSelectedApplication();
        }

        private void AddGameButton_Click(object sender, RoutedEventArgs e)
        {
            AddSelectedGame();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Executables|*.exe|All Files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.ExpandEnvironmentVariables("%ProgramW6432%");
            openFileDialog.Title = "Select Executable";

            if (openFileDialog.ShowDialog() == true)
            {
                ExecutablePath = openFileDialog.FileName;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string? base64Icon = ExecutableIcon != null ? ImageUtils.ImageToBase64((BitmapSource)ExecutableIcon) : null;
            List<SavedItem> savedItems = StorageUtils.ReadItemsFromFile();
            savedItems.Add(new SavedExecutable(ExecutableName, base64Icon, ExecutablePath));
            StorageUtils.SaveItemsToFile(savedItems);
            Close();
        }

        private void ApplicationListBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddSelectedApplication();
                e.Handled = true;
            }
        }

        private void AddSelectedApplication()
        {
            InstalledApplication? installedApplication = ApplicationListBox.SelectedItem as InstalledApplication;
            if (installedApplication != null)
            {
                List<SavedItem> savedItems = StorageUtils.ReadItemsFromFile();
                savedItems.Add(new SavedApplication(installedApplication));
                StorageUtils.SaveItemsToFile(savedItems);
                Close();
            }
        }

        private void AddSelectedGame()
        {
            InstalledGame? installedGame = GameListBox.SelectedItem as InstalledGame;
            if (installedGame != null)
            {
                List<SavedItem> savedItems = StorageUtils.ReadItemsFromFile();
                savedItems.Add(installedGame.SavedItem);
                StorageUtils.SaveItemsToFile(savedItems);
                Close();
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Activate();

            var installedApps = await Task.Run(() => GetInstalledApplications());
            SetInstalledApps(installedApps);

            var installedSteamGames = await Task.Run(() => GetInstalledSteamGames());
            InstalledGames = installedSteamGames;
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            new ShowMainWindowCommand().Execute(null);
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
    }
}
