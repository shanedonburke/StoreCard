using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace StoreCard
{
    /// <summary>
    /// Interaction logic for ChangeExecutableWindow.xaml
    /// </summary>
    public partial class ChangeExecutableWindow : INotifyPropertyChanged
    {
        private readonly SavedFileSystemItem _item;

        private string _appSearchText = "";

        private bool _areAppsLoaded;

        private List<InstalledApplication> _installedApps = new();

        private IEnumerable<InstalledApplication> _filteredApps = new List<InstalledApplication>();

        private bool _doesExecutableExist;

        private ImageSource? _executableIcon;

        private string _executableName = "";

        private string _executablePath = "";

        public IEnumerable<InstalledApplication> FilteredApps
        {
            get => _filteredApps;
            set
            {
                _filteredApps = value;
                OnPropertyChanged("FilteredApps");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (_filteredApps.Any()) AppListBox.SelectedIndex = 0;
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

        public bool AreAppsLoaded
        {
            get => _areAppsLoaded;
            set
            {
                _areAppsLoaded = value;
                OnPropertyChanged("AreAppsLoaded");
            }
        }

        public bool ShouldEnableSaveAppButton => AppListBox.SelectedIndex != -1;

        public string? SelectedAppName => (AppListBox.SelectedItem as InstalledApplication)?.Name;

        public ImageSource? SelectedAppIcon => (AppListBox.SelectedItem as InstalledApplication)?.BitmapIcon;

        public string ExecutablePath {
            get => _executablePath;
            set {
                _executablePath = value;

                DoesExecutableExist = File.Exists(value);
                if (DoesExecutableExist) {
                    // Take file name without '.exe'
                    ExecutableName = value.Split(@"\").Last();

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

        public string ExecutableName {
            get => _executableName;
            set {
                _executableName = value;
                OnPropertyChanged("ExecutableName");
            }
        }

        public bool DoesExecutableExist {
            get => _doesExecutableExist;
            set {
                _doesExecutableExist = value;
                OnPropertyChanged("DoesExecutableExist");
            }
        }

        public ImageSource? ExecutableIcon {
            get => _executableIcon;
            set {
                _executableIcon = value;
                OnPropertyChanged("ExecutableIcon");
            }
        }

        public ChangeExecutableWindow(SavedFileSystemItem item)
        {
            _item = item;
            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void SetInstalledApps(List<InstalledApplication> value)
        {
            _installedApps = value;
            _installedApps.Sort();
            AreAppsLoaded = true;
            FilteredApps = FilterApps();
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (_installedApps.Count > 0) AppListBox.SelectedIndex = 0;
            });
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

        private void AppSearchBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    if (AppListBox.Items.Count == 0) return;
                    switch (AppListBox.SelectedIndex)
                    {
                        case 0:
                        case -1:
                            AppListBox.SelectedIndex = AppListBox.Items.Count - 1;
                            break;
                        default:
                            AppListBox.SelectedIndex =
                                (AppListBox.SelectedIndex - 1) % AppListBox.Items.Count;
                            break;
                    }

                    AppListBox.ScrollIntoView(AppListBox.SelectedItem);
                    break;
                case Key.Down:
                    if (AppListBox.Items.Count == 0) return;
                    if (AppListBox.SelectedIndex == -1)
                        AppListBox.SelectedIndex = 0;
                    else
                        AppListBox.SelectedIndex =
                            (AppListBox.SelectedIndex + 1) % AppListBox.Items.Count;
                    AppListBox.ScrollIntoView(AppListBox.SelectedItem);
                    break;
            }
        }

        private void AppListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnPropertyChanged("ShouldEnableSaveAppButton");
            OnPropertyChanged("SelectedAppName");
            OnPropertyChanged("SelectedAppIcon");
        }

        private void SaveAppButton_Click(object sender, RoutedEventArgs e)
        {
            List<SavedItem> savedItems = StorageUtils.ReadItemsFromFile();
            var matchingItem = savedItems.Find(i => i.Id == _item.Id) as SavedFileSystemItem;
            if (matchingItem == null)
            {
                Debug.WriteLine("Tried to change item executable, but no matching stored item was found.");
                return;
            }

            matchingItem.SetExecutablePath((AppListBox.SelectedItem as InstalledApplication)?.ExecutablePath
                                           ?? SavedFileSystemItem.DEFAULT_EXECUTABLE);
            StorageUtils.SaveItemsToFile(savedItems);
            DialogResult = true;
            Close();
        }
        private void SaveDefaultButton_Click(object sender, RoutedEventArgs e) {
            List<SavedItem> savedItems = StorageUtils.ReadItemsFromFile();
            var matchingItem = savedItems.Find(i => i.Id == _item.Id) as SavedFileSystemItem;
            if (matchingItem == null) {
                Debug.WriteLine("Tried to change item executable, but no matching stored item was found.");
                return;
            }

            matchingItem.SetExecutablePath(SavedFileSystemItem.DEFAULT_EXECUTABLE);
            StorageUtils.SaveItemsToFile(savedItems);
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(() => SetInstalledApps(SystemUtils.GetInstalledApplications()));
        }

        private void SaveExecutableButton_Click(object sender, RoutedEventArgs e)
        {
            var savedItems = StorageUtils.ReadItemsFromFile();
            var matchingItem = savedItems.Find(i => i.Id == _item.Id) as SavedFileSystemItem;
            if (matchingItem == null) {
                Debug.WriteLine("Tried to change item executable, but no matching stored item was found.");
                return;
            }
            matchingItem.SetExecutablePath(ExecutablePath);
            StorageUtils.SaveItemsToFile(savedItems);
            DialogResult = true;
            Close();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog {
                Filter = "Executables|*.exe|All Files (*.*)|*.*",
                InitialDirectory = Environment.ExpandEnvironmentVariables("%ProgramW6432%"),
                Title = "Select Executable"
            };

            if (openFileDialog.ShowDialog() == true) ExecutablePath = openFileDialog.FileName;
        }
    }
}