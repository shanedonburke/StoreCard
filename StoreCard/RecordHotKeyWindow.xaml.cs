using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using StoreCard.Annotations;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace StoreCard
{
    /// <summary>
    /// Interaction logic for RecordHotKeyWindow.xaml
    /// </summary>
    public partial class RecordHotKeyWindow : INotifyPropertyChanged
    {
        private string _hotKeyText = "";
        private readonly UserConfig _config;

        public string HotKeyText
        {
            get => _hotKeyText;
            set
            {
                _hotKeyText = value;
                OnPropertyChanged(nameof(HotKeyText));
            }
        }
        public RecordHotKeyWindow() {
            InitializeComponent();
            _config = StorageUtils.ReadConfigFromFile();
            HotKeyText = HotKeyUtils.KeyStringFromConfig(_config);
            DataContext = this;
        }

        private void RecordHotKeyWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            var key = e.Key == Key.System ? e.SystemKey : e.Key;

            // Ignore modifier keys.
            if (key is Key.LeftShift or Key.RightShift or Key.LeftCtrl or Key.RightCtrl or Key.LeftAlt or Key.RightAlt or Key.LWin or Key.RWin) {
                return;
            }

            StringBuilder text = new();
            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0) {
                text.Append("Ctrl+");
            }
            if ((Keyboard.Modifiers & ModifierKeys.Alt) != 0) {
                text.Append("Alt+");
            }
            if ((Keyboard.Modifiers & ModifierKeys.Windows) != 0 || Keyboard.IsKeyDown(Key.LWin) || Keyboard.IsKeyDown(Key.RWin)) {
                text.Append("Win+");
            }
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0) {
                text.Append("Shift+");
            }

            text.Append(HotKeyUtils.KeyToString(key));
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
            HotKeyText = HotKeyUtils.KeyStringFromConfig(_config);
        }
    }
}
