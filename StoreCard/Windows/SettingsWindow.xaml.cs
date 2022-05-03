using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using StoreCard.Commands;
using StoreCard.Models;
using StoreCard.Properties;
using StoreCard.Services;
using StoreCard.Utils;
using StoreCard.Windows;

namespace StoreCard
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : INotifyPropertyChanged
    {

        private UserConfig _config;

        public string HotKeyText => HotKeyService.KeyStringFromConfig(_config);

        public bool IsStartupShortcutDisabled => SystemUtils.IsStartupShortcutEnabled() == false;

        public SettingsWindow() {
            InitializeComponent();
            _config = StorageUtils.ReadConfigFromFile();
            DataContext = this;

            RunOnStartupCheckBox.IsChecked = SystemUtils.IsStartupShortcutEnabled() != null;
        }

        private void SettingsWindow_Closed(object? sender, EventArgs e)
        {
            StorageUtils.SaveConfigToFile(_config);
            new ShowMainWindowCommand().Execute(null);
            Close();
        }

        private void RecordHotKeyButton_Click(object sender, RoutedEventArgs e)
        {
            if (new RecordHotKeyWindow().ShowDialog() != true) return;
            _config = StorageUtils.ReadConfigFromFile();
            OnPropertyChanged(nameof(HotKeyText));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RunOnStartupCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SystemUtils.CreateStartupShortcut();
            OnPropertyChanged(nameof(IsStartupShortcutDisabled));
        }

        private void RunOnStartupCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SystemUtils.RemoveStartupShortcut();
            OnPropertyChanged(nameof(IsStartupShortcutDisabled));
        }
    }
}
