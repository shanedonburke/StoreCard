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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using StoreCard.Models.Items.Installed;
using StoreCard.Models.Items.Saved;
using StoreCard.Properties;
using StoreCard.UserControls;
using StoreCard.Utils;

namespace StoreCard.Windows
{
    /// <summary>
    /// Interaction logic for ChangeExecutableWindow.xaml
    /// </summary>
    public partial class ChangeExecutableWindow : INotifyPropertyChanged
    {
        private readonly SavedFileSystemItem _item;

        private bool _doesExecutableExist;

        private ImageSource? _executableIcon;

        private string _executableName = "";

        public bool ShouldEnableSaveAppButton => AppListBox.SelectedItem != null;

        public string? SelectedAppName => (AppListBox.SelectedItem as InstalledApplication)?.Name;

        public ImageSource? SelectedAppIcon => (AppListBox.SelectedItem as InstalledApplication)?.BitmapIcon;

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
            var savedItems = AppData.ReadItemsFromFile();
            if (savedItems.Find(i => i.Id == _item.Id) is not SavedFileSystemItem matchingItem) {
                Debug.WriteLine("Tried to change item executable, but no matching stored item was found.");
                return;
            }

            matchingItem.SetExecutablePath(SavedFileSystemItem.DEFAULT_EXECUTABLE);
            AppData.SaveItemsToFile(savedItems);
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

        private void LoadApps() {
            foreach (var app in Applications.GetInstalledApplications()) {
                AppListBox.AddItem(app);
            }
            AppListBox.FinishAddingItems();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(LoadApps);
        }

        private void SaveExecutableButton_Click(object sender, RoutedEventArgs e)
        {
            var savedItems = AppData.ReadItemsFromFile();
            if (savedItems.Find(i => i.Id == _item.Id) is not SavedFileSystemItem matchingItem) {
                Debug.WriteLine("Tried to change item executable, but no matching stored item was found.");
                return;
            }
            matchingItem.SetExecutablePath(ExecutablePathBox.Text);
            AppData.SaveItemsToFile(savedItems);
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

            if (openFileDialog.ShowDialog() == true) ExecutablePathBox.Text = openFileDialog.FileName;
        }

        private void SaveSelectedAppAndClose() {
            var savedItems = AppData.ReadItemsFromFile();
            if (savedItems.Find(i => i.Id == _item.Id) is not SavedFileSystemItem matchingItem) {
                Debug.WriteLine("Tried to change item executable, but no matching stored item was found.");
                return;
            }

            matchingItem.SetExecutablePath((AppListBox.SelectedItem as InstalledApplication)?.ExecutablePath
                                           ?? SavedFileSystemItem.DEFAULT_EXECUTABLE);
            AppData.SaveItemsToFile(savedItems);
            DialogResult = true;
            Close();
        }

        private void ExecutablePathBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = ExecutablePathBox.Text;
            DoesExecutableExist = File.Exists(text);
            if (!DoesExecutableExist) return;
            // Take file name without '.exe'
            ExecutableName = text.Split(@"\").Last();

            var icon = System.Drawing.Icon.ExtractAssociatedIcon(text);
            if (icon != null)
                ExecutableIcon = Imaging.CreateBitmapSourceFromHIcon(
                    icon.Handle,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
        }
    }
}