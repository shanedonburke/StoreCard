using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using StoreCard.Models;
using StoreCard.Properties;
using StoreCard.Services;
using StoreCard.Utils;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace StoreCard.Windows
{
    /// <summary>
    /// Interaction logic for RecordHotKeyWindow.xaml
    /// </summary>
    public partial class RecordHotKeyWindow : INotifyPropertyChanged
    {
        private readonly UserConfig _config;
        private uint _modifiers;
        private uint _virtualKey;
        private string _hotKeyText = "";

        public string HotKeyText
        {
            get => _hotKeyText;
            set
            {
                _hotKeyText = value;
                OnPropertyChanged(nameof(HotKeyText));
            }
        }

        public RecordHotKeyWindow()
        {
            InitializeComponent();
            _config = AppData.ReadConfigFromFile();
            HotKeyText = HotKeys.KeyStringFromConfig(_config);
            DataContext = this;
        }

        private void RecordHotKeyWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            var key = e.Key == Key.System ? e.SystemKey : e.Key;

            // Ignore modifier keys.
            if (key is Key.LeftShift or Key.RightShift or Key.LeftCtrl or Key.RightCtrl or Key.LeftAlt or Key.RightAlt
                or Key.LWin or Key.RWin)
            {
                return;
            }

            _modifiers = 0;
            _virtualKey = 0;

            StringBuilder text = new();
            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {
                text.Append("Ctrl+");
                _modifiers |= (uint) ModifierKeys.Control;
            }

            if ((Keyboard.Modifiers & ModifierKeys.Alt) != 0)
            {
                text.Append("Alt+");
                _modifiers |= (uint) ModifierKeys.Alt;
            }

            if ((Keyboard.Modifiers & ModifierKeys.Windows) != 0 || Keyboard.IsKeyDown(Key.LWin) ||
                Keyboard.IsKeyDown(Key.RWin))
            {
                text.Append("Win+");
                _modifiers |= (uint) ModifierKeys.Windows;
            }

            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            {
                text.Append("Shift+");
                _modifiers |= (uint) ModifierKeys.Shift;
            }

            _virtualKey = HotKeys.KeyToVirtualKey(key);
            text.Append(HotKeys.KeyToString(key));
            HotKeyText = text.ToString();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            HotKeyText = HotKeys.KeyStringFromConfig(_config);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _config.HotKeyModifiers = _modifiers;
            _config.VirtualHotKey = _virtualKey;
            AppData.SaveConfigToFile(_config);
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}