using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using StoreCard.Annotations;

namespace StoreCard
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : INotifyPropertyChanged
    {

        private UserConfig _config;

        public string HotKeyText => HotKeyUtils.KeyStringFromConfig(_config);

        public SettingsWindow() {
            InitializeComponent();
            _config = StorageUtils.ReadConfigFromFile();
            DataContext = this;
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

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            SystemUtils.CreateStartupShortcut();
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            SystemUtils.RemoveStartupShortcut();
        }
    }
}
