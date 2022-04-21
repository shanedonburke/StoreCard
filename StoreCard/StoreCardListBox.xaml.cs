using System;
using System.Collections.Generic;
using System.Linq;
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

namespace StoreCard
{
    /// <summary>
    /// Interaction logic for StoreCardListBox.xaml
    /// </summary>
    public partial class StoreCardListBox
    {
        public static readonly RoutedEvent ItemDoubleClickEvent = EventManager.RegisterRoutedEvent(
            "ItemDoubleClick",
            RoutingStrategy.Bubble,
            typeof(MouseButtonEventHandler),
            typeof(StoreCardListBox));

        public new static readonly RoutedEvent PreviewKeyDownEvent = EventManager.RegisterRoutedEvent(
            "PreviewKeyDown",
            RoutingStrategy.Bubble,
            typeof(KeyEventHandler),
            typeof(StoreCardListBox));

        public static readonly DependencyProperty ShowActionButtonProperty = DependencyProperty.Register(
            nameof(ShowActionButton),
            typeof(bool),
            typeof(StoreCardListBox),
            new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty ActionButtonTextProperty = DependencyProperty.Register(
            nameof(ActionButtonText),
            typeof(string),
            typeof(StoreCardListBox),
            new FrameworkPropertyMetadata("Open"));

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
            nameof(Items),
            typeof(IEnumerable<object>),
            typeof(StoreCardListBox));

        public static readonly DependencyProperty ItemContextMenuProperty = DependencyProperty.Register(
            nameof(ItemContextMenu),
            typeof(ContextMenu),
            typeof(StoreCardListBox));

        public IEnumerable<object> Items
        {
            get => (IEnumerable<object>) GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        public ContextMenu ItemContextMenu
        {
            get => (ContextMenu) GetValue(ItemContextMenuProperty);
            set => SetValue(ItemContextMenuProperty, value);
        }

        public bool ShowActionButton
        {
            get => (bool) GetValue(ShowActionButtonProperty);
            set => SetValue(ShowActionButtonProperty, value);
        }

        public string ActionButtonText
        {
            get => (string) GetValue(ActionButtonTextProperty);
            set => SetValue(ActionButtonTextProperty, value);
        }

        public event MouseButtonEventHandler ItemDoubleClick
        {
            add => AddHandler(ItemDoubleClickEvent, value);
            remove => RemoveHandler(ItemDoubleClickEvent, value);
        }

        public new event KeyEventHandler PreviewKeyDown
        {
            add => AddHandler(PreviewKeyDownEvent, value);
            remove => RemoveHandler(PreviewKeyDownEvent, value);
        }

        public int SelectedIndex
        {
            get => ItemListBox.SelectedIndex;
            set => ItemListBox.SelectedIndex = value;
        }

        public object SelectedItem
        {
            get => ItemListBox.SelectedItem;
            set => ItemListBox.SelectedItem = value;
        }

        public StoreCardListBox() {
            InitializeComponent();
        }

        public void ScrollIntoView(object item)
        {
            ItemListBox.ScrollIntoView(item);
        }

        private void Item_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left) { RoutedEvent = ItemDoubleClickEvent });
            }
        }

        private void ItemListBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            RaiseEvent(new KeyEventArgs(Keyboard.PrimaryDevice, e.InputSource, e.Timestamp, e.Key) { RoutedEvent = PreviewKeyDownEvent });
        }
    }
}
