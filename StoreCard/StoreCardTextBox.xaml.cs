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
        public static readonly DependencyProperty ActivePlaceholderProperty = DependencyProperty.Register(
            nameof(ActivePlaceholder),
            typeof(string),
            typeof(StoreCardTextBox),
            new FrameworkPropertyMetadata(""));

        public static readonly DependencyProperty InactivePlaceholderProperty = DependencyProperty.Register(
            nameof(InactivePlaceholder),
            typeof(string),
            typeof(StoreCardTextBox),
            new FrameworkPropertyMetadata(""));

        public new static readonly RoutedEvent PreviewKeyDownEvent = EventManager.RegisterRoutedEvent(
            nameof(PreviewKeyDown),
            RoutingStrategy.Bubble,
            typeof(KeyEventHandler),
            typeof(StoreCardTextBox));

        public new static readonly RoutedEvent KeyUpEvent = EventManager.RegisterRoutedEvent(
            nameof(KeyUpEvent),
            RoutingStrategy.Bubble,
            typeof(KeyEventHandler),
            typeof(StoreCardTextBox));

        public static readonly RoutedEvent TextChangedEvent = EventManager.RegisterRoutedEvent(
            nameof(TextChanged),
            RoutingStrategy.Bubble,
            typeof(TextChangedEventHandler),
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

        public string ActivePlaceholder
        {
            get => (string) GetValue(ActivePlaceholderProperty);
            set => SetValue(ActivePlaceholderProperty, value);
        }

        public string InactivePlaceholder
        {
            get => (string) GetValue(InactivePlaceholderProperty);
            set => SetValue(InactivePlaceholderProperty, value);
        }

        public new event KeyEventHandler PreviewKeyDown
        {
            add => AddHandler(PreviewKeyDownEvent, value);
            remove => RemoveHandler(PreviewKeyDownEvent, value);
        }

        public new event KeyEventHandler KeyUp
        {
            add => AddHandler(KeyUpEvent, value);
            remove => RemoveHandler(KeyUpEvent, value);
        }

        public event TextChangedEventHandler TextChanged
        {
            add => AddHandler(TextChangedEvent, value);
            remove => RemoveHandler(TextChangedEvent, value);
        }

        public StoreCardTextBox()
        {
            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CustomTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            RaiseEvent(new KeyEventArgs(Keyboard.PrimaryDevice, e.InputSource, e.Timestamp, e.Key)
                {RoutedEvent = PreviewKeyDownEvent});
        }

        private void CustomTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            RaiseEvent(new TextChangedEventArgs(TextChangedEvent, e.UndoAction));
        }

        private void CustomTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RaiseEvent(new KeyEventArgs(Keyboard.PrimaryDevice, e.InputSource, e.Timestamp, e.Key) {RoutedEvent = KeyUpEvent});
            }
        }
    }
}
