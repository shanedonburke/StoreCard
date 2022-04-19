using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StoreCard.Annotations;

namespace StoreCard
{
    /// <summary>
    /// Interaction logic for EditFileWindow.xaml
    /// </summary>
    public partial class EditFileWindow : INotifyPropertyChanged
    {
        private SavedFileSystemItem _item;

        private string _itemName = "";

        public string Path => _item.Path;

        public string ItemName
        {
            get => _itemName;
            set
            {
                _itemName = value;
                OnPropertyChanged(nameof(ItemName));
                OnPropertyChanged(nameof(ShouldEnableSaveNameButton));
            }
        }

        public bool ShouldEnableSaveNameButton => ItemName.Trim() != "" && ItemName != _item.Name;

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

        public EditFileWindow(SavedFileSystemItem item) {
            _item = item;
            ItemName = item.Name;

            DataContext = this;
            InitializeComponent();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void ChangeExecutableButton_Click(object sender, RoutedEventArgs e)
        {
            if (new ChangeExecutableWindow(_item).ShowDialog() != true) return;
            var savedItems = StorageUtils.ReadItemsFromFile();
            if (savedItems.Find(i => i.Id == _item.Id) is not SavedFileSystemItem matchingItem)
            {
                Debug.WriteLine("Failed to find matching item for edit file window.");
                return;
            }
            _item = matchingItem;
            OnPropertyChanged(nameof(ExecutableName));
            OnPropertyChanged(nameof(ExecutableIcon));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Window_Closed(object? sender, EventArgs e)
        {
            new ShowMainWindowCommand().Execute(null);
        }

        private void SaveNameButton_Click(object sender, RoutedEventArgs e)
        {
            _item.Name = ItemName;
            var savedItems = StorageUtils.ReadItemsFromFile();
            var matchingItem = savedItems.Find(i => i.Id == _item.Id);
            if (matchingItem != null)
            {
                matchingItem.Name = ItemName;
                StorageUtils.SaveItemsToFile(savedItems);
                OnPropertyChanged(nameof(ShouldEnableSaveNameButton));
            }
            else
            {
                Debug.WriteLine("Tried to change item name, but no matching stored item was found.");
            }
        }
    }
}
