using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StoreCard
{
    /// <summary>
    /// Interaction logic for FileOptionsWindow.xaml
    /// </summary>
    public partial class FileOptionsWindow : INotifyPropertyChanged
    {
        private SavedFileSystemItem _item;

        public string ExecutableName => _item.ExecutableName;

        public ImageSource ExecutableIcon
        {
            get
            {
                var execPath = _item.ExecutablePath;
                if (!File.Exists(execPath))
                {
                    execPath = SavedFileSystemItem.DEFAULT_EXECUTABLE;
                }
                var icon = System.Drawing.Icon.ExtractAssociatedIcon(execPath) ?? System.Drawing.Icon.ExtractAssociatedIcon(SavedFileSystemItem.DEFAULT_EXECUTABLE);
                Debug.Assert(icon != null, nameof(icon) + " != null");
                return Imaging.CreateBitmapSourceFromHIcon(
                    icon.Handle,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
        }

        public FileOptionsWindow(SavedFileSystemItem item) {
            _item = item;
            DataContext = this;
            InitializeComponent();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void ChangeExecutableButton_Click(object sender, RoutedEventArgs e)
        {
            if (new ChangeExecutableWindow(_item).ShowDialog() == true)
            {
                List<SavedItem> savedItems = StorageUtils.ReadItemsFromFile();
                var matchingItem = savedItems.Find(i => i.Id == _item.Id) as SavedFileSystemItem;
                if (matchingItem == null)
                {
                    Debug.WriteLine("Failed to find matching item for file options window.");
                    return;
                }
                _item = matchingItem;
                OnPropertyChanged("ExecutableName");
                OnPropertyChanged("ExecutableIcon");
            }
        }
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void Window_Closed(object? sender, EventArgs e)
        {
            new ShowMainWindowCommand().Execute(null);
        }
    }
}
