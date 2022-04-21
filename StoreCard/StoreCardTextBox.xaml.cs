using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using StoreCard.Annotations;

namespace StoreCard
{
    /// <summary>
    /// Interaction logic for StoreCardTextBox.xaml
    /// </summary>
    public partial class StoreCardTextBox : INotifyPropertyChanged
    {
        public static readonly RoutedEvent PreviewKeyDownEvent = EventManager.RegisterRoutedEvent(
            "PreviewKeyDown",
            RoutingStrategy.Bubble,
            typeof(KeyEventHandler),
            typeof(StoreCardTextBox));

        private string _text = "";

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        public new event KeyEventHandler PreviewKeyDown {
            add => AddHandler(PreviewKeyDownEvent, value);
            remove => RemoveHandler(PreviewKeyDownEvent, value);
        }

        public StoreCardTextBox() {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CustomTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            RaiseEvent(new KeyEventArgs(Keyboard.PrimaryDevice, e.InputSource, e.Timestamp, e.Key) { RoutedEvent = PreviewKeyDownEvent });
        }
    }
}
