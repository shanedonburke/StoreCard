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
using StoreCard.Annotations;

namespace StoreCard
{
    /// <summary>
    /// Interaction logic for ChangeExecutableWindow.xaml
    /// </summary>
    public partial class ChangeExecutableWindow : INotifyPropertyChanged
    {
        private readonly SavedFileSystemItem _item;

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
                OnPropertyChanged(nameof(FilteredApps));
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (_filteredApps.Any()) AppListBox.SelectedIndex = 0;
                });
            }
        }

        public bool ShouldEnableSaveAppButton => AppListBox.SelectedItem != null;

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

                OnPropertyChanged(nameof(ExecutablePath));
            }
        }

        public string ExecutableName {
            get => _executableName;
            set {
                _executableName = value;
                OnPropertyChanged(nameof(ExecutableName));
            }
        }

        public bool DoesExecutableExist {
            get => _doesExecutableExist;
            set {
                _doesExecutableExist = value;
                OnPropertyChanged(nameof(DoesExecutableExist));
            }
        }

        public ImageSource? ExecutableIcon {
            get => _executableIcon;
            set {
                _executableIcon = value;
                OnPropertyChanged(nameof(ExecutableIcon));
            }
        }

        public ChangeExecutableWindow(SavedFileSystemItem item)
        {
            _item = item;
            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void AppListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(ShouldEnableSaveAppButton));
            OnPropertyChanged(nameof(SelectedAppName));
            OnPropertyChanged(nameof(SelectedAppIcon));
        }


        private void AppListBox_ItemActivated(object sender, ItemActivatedEventArgs e) {
            SaveSelectedAppAndClose();
            e.Handled = true;
        }

        private void SaveAppButton_Click(object sender, RoutedEventArgs e)
        {
            SaveSelectedAppAndClose();
        }

        private void SaveDefaultButton_Click(object sender, RoutedEventArgs e) {
            var savedItems = StorageUtils.ReadItemsFromFile();
            if (savedItems.Find(i => i.Id == _item.Id) is not SavedFileSystemItem matchingItem) {
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

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(() => AppListBox.Items = SystemUtils.GetInstalledApplications());
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

        private void SaveSelectedAppAndClose() {
            var savedItems = StorageUtils.ReadItemsFromFile();
            if (savedItems.Find(i => i.Id == _item.Id) is not SavedFileSystemItem matchingItem) {
                Debug.WriteLine("Tried to change item executable, but no matching stored item was found.");
                return;
            }

            matchingItem.SetExecutablePath((AppListBox.SelectedItem as InstalledApplication)?.ExecutablePath
                                           ?? SavedFileSystemItem.DEFAULT_EXECUTABLE);
            StorageUtils.SaveItemsToFile(savedItems);
            DialogResult = true;
            Close();
        }
    }
}