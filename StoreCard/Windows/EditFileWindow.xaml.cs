using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StoreCard.Commands;
using StoreCard.Models.Items.Saved;
using StoreCard.Properties;
using StoreCard.Utils;

namespace StoreCard.Windows
{
    /// <summary>
    /// Interaction logic for EditFileWindow.xaml
    /// </summary>
    public partial class EditFileWindow : INotifyPropertyChanged
    {
        public EditFileWindow(SavedFileSystemItem item)
        {
            _item = item;

            DataContext = this;
            InitializeComponent();

            NameBox.Text = item.Name;
        }

        private SavedFileSystemItem _item;

        public string Path => _item.Path;

        public bool ShouldEnableSaveNameButton => NameBox.Text.Trim() != "" && NameBox.Text != _item.Name;

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

                var icon = System.Drawing.Icon.ExtractAssociatedIcon(execPath) ??
                           System.Drawing.Icon.ExtractAssociatedIcon(SavedFileSystemItem.DEFAULT_EXECUTABLE);
                Debug.Assert(icon != null, nameof(icon) + " != null");
                return Imaging.CreateBitmapSourceFromHIcon(
                    icon.Handle,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void ChangeExecutableButton_Click(object sender, RoutedEventArgs e)
        {
            if (new ChangeExecutableWindow(_item).ShowDialog() != true) return;

            var item = AppData.FindSavedItemById<SavedFileSystemItem>(AppData.ReadItemsFromFile(), _item.Id);
            if (item == null)
            {
                Debug.WriteLine("Failed to find matching item for edit file window.");
                return;
            }

            _item = item;
            OnPropertyChanged(nameof(ExecutableName));
            OnPropertyChanged(nameof(ExecutableIcon));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Window_Closed(object? sender, EventArgs e)
        {
            new ShowMainWindowCommand().Execute();
        }

        private void SaveNameButton_Click(object sender, RoutedEventArgs e)
        {
            var name = NameBox.Text;

            var updatedItem = AppData.UpdateSavedItemById<SavedFileSystemItem>(_item.Id, i => i.Name = name);

            if (updatedItem != null)
            {
                _item = updatedItem;
                OnPropertyChanged(nameof(ShouldEnableSaveNameButton));
            }
            else
            {
                Debug.WriteLine("Tried to change item name, but no matching stored item was found.");
            }
        }

        private void StoreCardTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            OnPropertyChanged(nameof(ShouldEnableSaveNameButton));
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            AppData.DeleteItemAndSave(_item);
            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}